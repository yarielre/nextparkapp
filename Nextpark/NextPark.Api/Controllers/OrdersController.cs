using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        public async Task<IActionResult> RenovateOrder(int id,[FromBody] OrderModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest("Invalid Model State parameter");

            try
            {
                var user = await _useRepository.FirstOrDefaultWhereAsync(u => u.Id == model.UserId).ConfigureAwait(false);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                
                 if (user.Balance < model.Price)
                 return BadRequest("Not enough money");
                 var order = _mapper.Map<OrderModel, Order>(model);
                _orderRepository.Update(order);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Order, OrderModel>(order);
                return Ok(vm);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
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
                    return BadRequest("Order not found.");
                var parking = _parkingRepository.Find(order.ParkingId);
                if (parking == null)
                {
                    return BadRequest("Parking not found.");
                }
                //Parking's owned user
                var parkingOwnedUser = _useRepository.Find(parking.UserId);
                if (parkingOwnedUser == null)
                {
                    return BadRequest("Parking's owned user not found.");
                }
                var tax = await _feedRepository.FirstOrDefaultWhereAsync(f => f.Name == "RentEarningPercent").ConfigureAwait(false);
                if (tax == null)
                {
                    return BadRequest("Rent earning tax not found.");
                }
                //User who created the order
                var userOrder = _useRepository.Find(order.UserId);
                if (userOrder == null)
                {
                    return BadRequest("User who rented the order not found");
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
                    TransactionId = new Guid(),
                    Type = TransactionType.RentTrasaction
                };

                var feedTransaction = new Transaction
                {
                    CashMoved = rentEraningTax,
                    UserId = parking.UserId,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Completed,
                    TransactionId = new Guid(),
                    Type = TransactionType.FeedTransaction
                };
               
                //Saving rent and feed transactions
                _transactionRepository.Add(renTransaction);
                _transactionRepository.Add(feedTransaction);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                //Update user balance when rent is finished
                userOrder.Balance = userOrder.Balance - order.Price;
                _useRepository.Update(userOrder);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                //Update user profit when rent is finished
                parkingOwnedUser.Profit = parkingOwnedUser.Profit + rentEraningTax;
                _useRepository.Update(parkingOwnedUser);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                //Update parking after status change
                _parkingRepository.Update(parking);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                //Update order after status change
                _orderRepository.Update(order);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                var vm = _mapper.Map<Order, OrderModel>(order);
                return Ok(vm);
            }
            catch (Exception e)
            {
                return BadRequest($"Server error: {e}");
            }
        }

        // GET api/controller
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _orderRepository.FindAllAsync().ConfigureAwait(false);
            var vm = _mapper.Map<List<Order>, List<OrderModel>>(list);
            return Ok(vm);
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _orderRepository.Find(id);
            if (entity == null)
                return BadRequest("Entity not found");
            var vm = _mapper.Map<Order, OrderModel>(entity);
            return Ok(vm);
        }

        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderModel orderModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parking = await _parkingRepository.FirstOrDefaultWhereAsync(p => p.Id == orderModel.ParkingId,
                new CancellationToken(), p => p.Events).ConfigureAwait(false);

            if (parking == null)
                return NotFound(ApiResponse.GetErrorResponse("Parking not found", ErrorType.ParkingNotFound));

            var isAvailable = IsParkingAvailable(parking, orderModel);

            if (!isAvailable)
                return BadRequest("Parking is not available");

            var parkigOrders = await _orderRepository.FindAllWhereAsync(ev => ev.ParkingId == orderModel.ParkingId);
            var isOrderdable= IsParkingOrderable(parkigOrders, orderModel);

            if (!isOrderdable)
                return BadRequest(ApiResponse.GetErrorResponse("Parking is not orderable", ErrorType.ParkingNotOrderable));

            var user = _useRepository.Find(orderModel.UserId);

            if (user.Balance < orderModel.Price)
                return BadRequest(ApiResponse.GetErrorResponse("Not enough money", ErrorType.NotEnoughMoney));

            try
            {
                var order = _mapper.Map<OrderModel, Order>(orderModel);
                _orderRepository.Add(order);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Order, OrderModel>(order);
                return Ok(ApiResponse.GetSuccessResponse(vm, "Order created"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OrderModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var entity = _mapper.Map<OrderModel, Order>(model);

            _orderRepository.Update(entity);


            var vm = _mapper.Map<Order, OrderModel>(entity);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            return Ok(vm);

        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _orderRepository.Find(id);
            if (entity == null)
                return BadRequest("Can't deleted, entity not found.");
            var vm = _mapper.Map<Order, OrderModel>(entity);
            _orderRepository.Delete(entity);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);
            return Ok(vm);
        }

        private bool IsParkingAvailable(Parking parking, OrderModel order)
        {
            //If parking have events, is enable and the date of the last event y bigger than order end date
            if (parking.Events == null) return false;
            if (parking.Events.Count == 0) return false;
            if (parking.Status != ParkingStatus.Enabled) return false;

            //Add logic to: Get the parking events of the same order date
            var orderDayEvents = parking.Events.Where(ev => ev.StartDate.Date == order.StartDate.Date).ToList();
            if (orderDayEvents == null || orderDayEvents.Count == 0) return false;

            var eventsStartingOnTime = orderDayEvents.Where(ev => ev.StartDate.TimeOfDay <= order.StartDate.TimeOfDay).ToList();
            if (eventsStartingOnTime == null || eventsStartingOnTime.Count == 0) return false;

            var eventsEndingOnTime = eventsStartingOnTime.Where(ev => ev.EndDate.TimeOfDay >= order.EndDate.TimeOfDay).ToList();
            if (eventsEndingOnTime == null || eventsEndingOnTime.Count == 0) return false;

            return true;

            //return parking.Events.Count > 0 && parking.Status == ParkingStatus.Enabled
            //       && order.EndDate <= parking.Events.OrderBy(e => e.EndDate).Last().EndDate;
        }

        private bool IsParkingOrderable(List<Order> parkingOrders, OrderModel order)
        {
            //If parking have events, is enable and the date of the last event y bigger than order end date
            if (parkingOrders == null) return true;
            if (parkingOrders.Count == 0) return true;

            //Add logic to: Check if the order start time bigger than any of the events start time.
            //Add logic to: Check if the order end time is smaller or equal to the same event end time.

            //Add logic to: Get the parking events of the same order date
            var orderDayEvents = parkingOrders.Where(ev => ev.StartDate.Date == order.StartDate.Date).ToList();
            if (orderDayEvents == null || orderDayEvents.Count == 0) return true;

            var eventsStartingOnTime = orderDayEvents.Where(ev => ev.StartDate.TimeOfDay <= order.StartDate.TimeOfDay).ToList();
            if (eventsStartingOnTime == null || eventsStartingOnTime.Count == 0) return true;

            var eventsEndingOnTime = eventsStartingOnTime.Where(ev => ev.EndDate.TimeOfDay >= order.EndDate.TimeOfDay).ToList();
            if (eventsEndingOnTime == null || eventsEndingOnTime.Count == 0) return true;

            return false;
        }

        private double CalCulateTax(double price,double taxPorcent)
        {
            return (price * taxPorcent) / 100;
        }
    }
}