using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : BaseController<Order, OrderModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IRepository<ApplicationUser> _useRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IRepository<Order> repository,
                                IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IUnitOfWork unitOfWork1,
                                IRepository<Parking> parkingRepository,
                                IRepository<Order> orderRepository,
                                IMapper mapper1,
                                IRepository<ApplicationUser> useRepository)
            : base(repository, unitOfWork, mapper)
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
            {
                return BadRequest("Invalid RenovateOrder parameter");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Model State parameter");
            }

            try
            {

                var order = _mapper.Map<OrderModel, Order>(model.Order);
                _orderRepository.Update(order);
                var user = _useRepository.Find(model.User.Id);
                user.Coins = model.User.Coins;
                _useRepository.Update(user);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Order, OrderModel>(order);
                return Ok(vm);

            }
            catch (Exception e)
            {

                return BadRequest(string.Format("Server error: {0}", e));
            }


        }

        [HttpPost("terminate")]
        public async Task<IActionResult> TeminateOrder([FromBody]int id)
        {

            try
            {

                var order = _orderRepository.Find(id);

                if (order == null)
                {
                    return BadRequest("Order not found.");
                }

                _orderRepository.Delete(order);
                var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId, new CancellationToken(),
                    p => p.ParkingCategory, p => p.ParkingType, p => p.ParkingEvent);
                parking.IsRented = false;
                _parkingRepository.Update(parking);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Parking, ParkingModel>(parking);
                return Ok(vm);

            }
            catch (Exception e)
            {

                return BadRequest(string.Format("Server error: {0}", e));
            }



        }
    }
}