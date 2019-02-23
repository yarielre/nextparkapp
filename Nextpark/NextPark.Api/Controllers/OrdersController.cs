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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationUser> _useRepository;

        public OrdersController(IRepository<Order> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUnitOfWork unitOfWork1,
            IRepository<Parking> parkingRepository,
            IRepository<Order> orderRepository,
            IMapper mapper1,
            IRepository<ApplicationUser> useRepository)
        {
            _unitOfWork = unitOfWork1;
            _parkingRepository = parkingRepository;
            _orderRepository = orderRepository;
            _mapper = mapper1;
            _useRepository = useRepository;
        }

        [HttpPost("renew")]
        public async Task<IActionResult> RenovateOrder([FromBody] RenovateOrder model)
        {
            if (model == null)
                return BadRequest("Invalid RenovateOrder parameter");

            if (!ModelState.IsValid)
                return BadRequest("Invalid Model State parameter");

            try
            {
                var order = _mapper.Map<OrderModel, Order>(model.Order);
                _orderRepository.Update(order);
                var user = _useRepository.Find(model.User.Id);
                user.Balance = model.User.Balance;
                _useRepository.Update(user);
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
                var order = _orderRepository.Find(id);

                if (order == null)
                    return BadRequest("Order not found.");

                _orderRepository.Delete(order);
                var parking = await _parkingRepository
                    .FirstWhereAsync(p => p.Id == order.ParkingId).ConfigureAwait(false);
                _parkingRepository.Update(parking);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Parking, ParkingModel>(parking);
                return Ok(vm);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
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
                return NotFound("Not found parking");
            var isAvailable = IsParkingAvailable(parking, orderModel);
            if (!isAvailable)
                return BadRequest("Parking is not available");
            var user = _useRepository.Find(orderModel.UserId);
            if (user.Balance < orderModel.Price)
                return BadRequest("Not enough money");
            try
            {
                var order = _mapper.Map<OrderModel, Order>(orderModel);
                _orderRepository.Add(order);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                try
                {
                    user.Balance = user.Balance - orderModel.Price;
                    _useRepository.Update(user);
                    await _unitOfWork.CommitAsync().ConfigureAwait(false);
                }
                // if fail the user update after substarct the order's price form user's balance then the order 
                // will be erased.
                catch (Exception e)
                {
                    _orderRepository.Delete(order);
                    await _unitOfWork.CommitAsync().ConfigureAwait(false);
                    return BadRequest(e.Message);
                }
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Order, OrderModel>(order);
                return Ok(vm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Order entity)
        {
            if (ModelState.IsValid)
            {
                _orderRepository.Update(entity);
                var vm = _mapper.Map<Order, OrderModel>(entity);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                return Ok(vm);
            }
            return BadRequest(ModelState);
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
            return parking.Events.Count > 0 && parking.Status == ParkingStatus.Enabled
                   && order.EndDate <= parking.Events.OrderBy(e => e.EndDate).Last().EndDate;
        }
    }
}