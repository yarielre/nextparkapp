using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Enums.Enums;
using NextPark.Models;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IRepository<Feed> _feedRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationUser> _useRepository;

        public OrdersController(IRepository<Order> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IRepository<Parking> parkingRepository,
            IRepository<Order> orderRepository,
            IRepository<ApplicationUser> useRepository,
            IRepository<Feed> feedRepository,
            IRepository<Transaction> transactionRepository)
        {
            _unitOfWork = unitOfWork;
            _parkingRepository = parkingRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _useRepository = useRepository;
            _feedRepository = feedRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPut("{id}/renew")]
        public async Task<IActionResult> RenovateOrder(int id, [FromBody] OrderModel model)
        {

            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Model is null", ErrorType.EntityNull));
            }

            try
            {
                Order currentOrder = _orderRepository.Find(model.Id);
                if (currentOrder == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Order not found", ErrorType.EntityNotFound));
                }

                Parking parking = await _parkingRepository.FirstOrDefaultWhereAsync(p => p.Id == model.ParkingId,
                    new CancellationToken(), p => p.Events).ConfigureAwait(false);

                if (parking == null)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));

                bool isAvailable = IsParkingAvailable(parking, model);

                if (!isAvailable)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking is not available", ErrorType.ParkingNotVailable));

                // Get all overlapped orders except this
                List<Order> overlappedOrders = await _orderRepository.FindAllWhereAsync(o => o.ParkingId == model.ParkingId &&
                                                                                o.Id != model.Id &&
                                                                                o.EndDate > model.StartDate &&
                                                                                o.StartDate < model.EndDate);

                if (overlappedOrders.Count > 0)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking is not orderable", ErrorType.ParkingNotOrderable));

                ApplicationUser user = await _useRepository.FirstOrDefaultWhereAsync(u => u.Id == model.UserId).ConfigureAwait(false);
                if (user == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("User not found",ErrorType.EntityNotFound));
                }

                double userAmountToPay = model.Price;

                // Get all user's active orders except this
                List<Order> userActiveOrders = await _orderRepository.FindAllWhereAsync(o => o.UserId == user.Id &&
                                                                                o.Id != model.Id &&
                                                                                o.OrderStatus == OrderStatus.Actived);
                foreach (Order order in userActiveOrders)
                {
                    // Sum the price of all user active orders
                    userAmountToPay += order.Price;
                }

                // Check if the user has enough money to pay all his active orders
                if (user.Balance < userAmountToPay)
                    return BadRequest(ApiResponse.GetErrorResponse("Not enough money", ErrorType.NotEnoughMoney));

                // Update the order instance
                Order updatedOrder = currentOrder;
                updatedOrder.StartDate = model.StartDate;
                updatedOrder.EndDate = model.EndDate;
                updatedOrder.Price = model.Price;

                _orderRepository.Update(updatedOrder);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                OrderModel vm = _mapper.Map<Order, OrderModel>(updatedOrder);
                return Ok(ApiResponse.GetSuccessResponse(vm));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse($"Server error: {e.Message}", ErrorType.Exception));
            }
        }

        [HttpPost("terminate")]
        public async Task<IActionResult> TeminateOrder([FromBody] int id)
        {
            try
            {
                //Logic terminate
                //Change order state  completed
                //Scale user balance
                //Change parking state to available

                var order = _orderRepository.Find(id);

                if (order == null)
                    return BadRequest(ApiResponse.GetErrorResponse("Order not found.",ErrorType.EntityNotFound));
                var parking = _parkingRepository.Find(order.ParkingId);
                if (parking == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Parking not found.",ErrorType.EntityNull));
                }
                //Parking's owned user
                var parkingOwnedUser = _useRepository.Find(parking.UserId);
                if (parkingOwnedUser == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Parking's owned user not found.",ErrorType.EntityNotFound));
                }
                var tax = await _feedRepository.FirstOrDefaultWhereAsync(f => f.Name == "RentEarningPercent").ConfigureAwait(false);
                if (tax == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Rent earning tax not found.",ErrorType.EntityNotFound));
                }
                //User who created the order
                var userOrder = _useRepository.Find(order.UserId);
                if (userOrder == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("User who rented the order not found",ErrorType.EntityNotFound));
                }

                var rentEraningTax = CalCulateTax(order.Price, tax.Tax);
                order.OrderStatus = OrderStatus.Finished;
                parking.Status = ParkingStatus.Enabled;

                var renTransaction = new Transaction
                {
                    CashMoved = rentEraningTax,
                    UserId = parking.UserId,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Completed,
                    TransactionId = Guid.NewGuid(),
                    Type = TransactionType.RentTransaction
                };

                var feedTransaction = new Transaction
                {
                    CashMoved = rentEraningTax,
                    UserId = parking.UserId,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Completed,
                    TransactionId = Guid.NewGuid(),
                    Type = TransactionType.FeedTransaction
                };

                //Saving rent and feed transactions
                _transactionRepository.Add(renTransaction);
                _transactionRepository.Add(feedTransaction);

                //Update user balance when rent is finished
                userOrder.Balance = userOrder.Balance - order.Price;
                _useRepository.Update(userOrder);

                //Update user profit when rent is finished
                parkingOwnedUser.Profit = parkingOwnedUser.Profit + rentEraningTax;
                _useRepository.Update(parkingOwnedUser);

                //Update parking after status change
                _parkingRepository.Update(parking);

                //Update order after status change
                _orderRepository.Update(order);


                await _unitOfWork.CommitAsync().ConfigureAwait(false);


                var vm = _mapper.Map<Order, OrderModel>(order);
                return Ok(ApiResponse.GetSuccessResponse(vm));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse($"Server error: {e.Message}",ErrorType.Exception));
            }
        }

        // GET api/controller
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _orderRepository.FindAllAsync().ConfigureAwait(false);
            var vm = _mapper.Map<List<Order>, List<OrderModel>>(list);
            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _orderRepository.Find(id);
            if (entity == null)
                return BadRequest(ApiResponse.GetErrorResponse("Entity not found",ErrorType.EntityNotFound));
            var vm = _mapper.Map<Order, OrderModel>(entity);
            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderModel orderModel)
        {
            if (orderModel == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Model is null", ErrorType.EntityNull));
            }

            try
            {            
                var parking = await _parkingRepository.FirstOrDefaultWhereAsync(p => p.Id == orderModel.ParkingId,
                    new CancellationToken(), p => p.Events).ConfigureAwait(false);

                if (parking == null)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));

                var isAvailable = IsParkingAvailable(parking, orderModel);

                if (!isAvailable)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking is not available", ErrorType.ParkingNotVailable));

                var overlappedOrders = await _orderRepository.FindAllWhereAsync(o => o.ParkingId == orderModel.ParkingId &&
                                                                                 o.EndDate > orderModel.StartDate &&
                                                                                 o.StartDate < orderModel.EndDate);

                if (overlappedOrders.Count > 0)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking is not orderable", ErrorType.ParkingNotOrderable));

                var user = _useRepository.Find(orderModel.UserId);

                double userAmountToPay = orderModel.Price;

                // Get all user's active orders
                var userActiveOrders = await _orderRepository.FindAllWhereAsync(o => o.UserId == user.Id &&
                                                                                o.OrderStatus == OrderStatus.Actived);
                foreach (Order order in userActiveOrders)
                {
                    // Sum the price of all user active orders
                    userAmountToPay += order.Price;
                }

                // Check if the user has enough money to pay all his active orders
                if (user.Balance < userAmountToPay)
                    return BadRequest(ApiResponse.GetErrorResponse("Not enough money", ErrorType.NotEnoughMoney));


                var entityOrder = _mapper.Map<OrderModel, Order>(orderModel);
                _orderRepository.Add(entityOrder);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Order, OrderModel>(entityOrder);
                return Ok(ApiResponse.GetSuccessResponse(vm, "Order created"));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse(e.Message,ErrorType.Exception));
            }
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrderModel model)
        {
            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Model is null", ErrorType.EntityNull));
            }

            try
            {
                Order currentOrder = _orderRepository.Find(model.Id);
                if (currentOrder == null) {
                    return BadRequest(ApiResponse.GetErrorResponse("Order not found", ErrorType.EntityNotFound));
                }

                Parking parking = await _parkingRepository.FirstOrDefaultWhereAsync(p => p.Id == model.ParkingId,
                    new CancellationToken(), p => p.Events).ConfigureAwait(false);

                if (parking == null)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));

                bool isAvailable = IsParkingAvailable(parking, model);

                if (!isAvailable)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking is not available", ErrorType.ParkingNotVailable));

                // Get all overlapped orders except this
                List<Order> overlappedOrders = await _orderRepository.FindAllWhereAsync(o => o.ParkingId == model.ParkingId &&
                                                                                o.Id != model.Id &&
                                                                                o.EndDate > model.StartDate &&
                                                                                o.StartDate < model.EndDate);

                if (overlappedOrders.Count > 0)
                    return BadRequest(ApiResponse.GetErrorResponse("Parking is not bookable", ErrorType.ParkingNotOrderable));

                ApplicationUser user = _useRepository.Find(model.UserId);

                double userAmountToPay = model.Price;

                // Get all user's active orders except this
                List<Order> userActiveOrders = await _orderRepository.FindAllWhereAsync(o => o.UserId == user.Id &&
                                                                                o.Id != model.Id &&
                                                                                o.OrderStatus == OrderStatus.Actived);
                foreach (Order order in userActiveOrders)
                {
                    // Sum the price of all user active orders
                    userAmountToPay += order.Price;
                }

                // Check if the user has enough money to pay all his active orders
                if (user.Balance < userAmountToPay)
                    return BadRequest(ApiResponse.GetErrorResponse("Not enough money", ErrorType.NotEnoughMoney));

                // Update the same unmuted order instance
                Order updatedOrder = currentOrder;
                updatedOrder.StartDate = model.StartDate;
                updatedOrder.EndDate = model.EndDate;
                updatedOrder.Price = model.Price;

                _orderRepository.Update(updatedOrder);

                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                OrderModel entityOrder = _mapper.Map<Order, OrderModel>(updatedOrder);
                return Ok(ApiResponse.GetSuccessResponse(entityOrder, "Order Updated"));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse(e.Message, ErrorType.Exception));
            }
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _orderRepository.Find(id);
            if (entity == null)
                return BadRequest(ApiResponse.GetErrorResponse("Can't deleted, entity not found.",ErrorType.EntityNotFound));
            var vm = _mapper.Map<Order, OrderModel>(entity);
            _orderRepository.Delete(entity);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);
            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        private bool IsParkingAvailable(Parking parking, OrderModel order)
        {
            //If parking have events, is enable and the date of the last event is bigger than order end date
            if (parking.Events == null) return false;
            if (parking.Events.Count == 0) return false;
            if (parking.Status != ParkingStatus.Enabled) return false;

            // Sort events by start date
            parking.Events.Sort((a, b) => (a.StartDate.CompareTo(b.StartDate)));

            // Filter only events that ends after the order starts
            var futureEvents = parking.Events.Where(ev => ev.EndDate > order.StartDate);

            // Check availability
            DateTime tempStart = order.StartDate;
            foreach (Event entityEvent in futureEvents) {
                if ((entityEvent.StartDate <= tempStart) && (entityEvent.EndDate > tempStart))
                {
                    if (entityEvent.EndDate >= order.EndDate)
                    {
                        // Availability found
                        return true;
                    }
                    else
                    {
                        // Search for contiguous next event
                        tempStart = entityEvent.EndDate.AddMinutes(1);
                    }
                }
            }
            return false;
        }

        private double CalCulateTax(double price, double taxPorcent)
        {
            return (price * taxPorcent) / 100;
        }
    }
}