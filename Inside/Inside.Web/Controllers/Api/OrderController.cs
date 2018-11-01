using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using Inside.Web.Infrastructure;
using Inside.Web.Models;
using Inside.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers.Api
{
    [Route("api/Order")]
    public class OrderController : BaseController<Order,OrderModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IRepository<ApplicationUser> _useRepository;
        private readonly IUnitOfWork _unitOfWork;
      

        public OrderController(IRepository<Order> repository, IUnitOfWork unitOfWork, IMapper mapper, IUnitOfWork unitOfWork1, IRepository<Parking> parkingRepository, IRepository<Order> orderRepository, IMapper mapper1, IRepository<ApplicationUser> useRepository) : base(repository, unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork1;
            _parkingRepository = parkingRepository;
            _orderRepository = orderRepository;
            _mapper = mapper1;
            _useRepository = useRepository;
        }

        [HttpPost("renovateorder")]
        public async Task<IActionResult> RenovateOrder([FromBody] RenovateOrder model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var order = _mapper.Map<OrderModel, Order>(model.Order);
            _orderRepository.Update(order);
            var user = _useRepository.Find(model.User.Id);
            user.Coins = model.User.Coins;
            _useRepository.Update(user);
            await _unitOfWork.CommitAsync();
            var vm = _mapper.Map<Order, OrderModel>(order);
            return Ok(vm);
        }

        [HttpPost("terminateorder")]
        public async Task<IActionResult> TeminateOrder([FromBody]int id)
        {
            var order = _orderRepository.Find(id);
            if (order == null)
            {
                return BadRequest("Order not found.");
            }
            _orderRepository.Delete(order);
            var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId,new CancellationToken() ,
                p => p.ParkingCategory, p => p.ParkingType, p => p.ParkingEvent);
            parking.IsRented = false;
            _parkingRepository.Update(parking);
            await _unitOfWork.CommitAsync();
            var vm = _mapper.Map<Parking, ParkingModel>(parking);
            return Ok(vm);
        }
    }
}