using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Models;
using NextPark.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParkingsController : ControllerBase
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
     
        private readonly IRepository<Event> _parkingEventRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IMediaService _mediaService;

        public ParkingsController(IRepository<Parking> repository, IUnitOfWork unitOfWork, IMapper mapper,
           IRepository<Order> orderRepository, 
           IRepository<Event> parkingEventRepository,
           IHostingEnvironment appEnvironment,
           IEmailSender emailSender,
           IMediaService mediaService)
        {
            _parkingRepository = repository;
            _orderRepository = orderRepository;
            _appEnvironment = appEnvironment;
            _parkingEventRepository = parkingEventRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _mediaService = mediaService;

        }

        // GET api/controller
        [HttpGet]
        public  async Task<IActionResult> Get()
        {
            var parkigns = await _parkingRepository.FindAllAsync(new CancellationToken());

            //TODO: Quitar toda logica de negocio de los conroladores a no ser que sea estrctamente necesario. 
            //Utilizar el backend lo mas posible como DATA CENTER!

            //PROBLEMA DE LA VISUALIZACION DE PARQUEOS!
            //if (parkigns.Count > 0)
            //    foreach (var park in parkigns)
            //        if (park.IsRented)
            //        {
            //            var rent = await _orderRepository.FirstOrDefaultWhereAsync(o =>
            //                o.ParkingId == park.Id && o.OrderStatus == OrderStatus.Actived);
            //        }

            var vm = _mapper.Map<List<Parking>, List<ParkingModel>>(parkigns);
            return Ok(vm);
        }
       
        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _parkingRepository.Find(id);
            if (entity == null)
                return BadRequest("Entity not found");
            var vm = _mapper.Map<Parking, ParkingModel>(entity);
            return Ok(vm);
        }
        
        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ParkingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _mediaService.SaveParkingImage(model);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
            }

            try
            {
                var parking = _mapper.Map<ParkingModel, Parking>(model);
                _parkingRepository.Add(parking);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Parking, ParkingModel>(parking);
                return Ok(vm);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("{0} Exception: {1}", "Error saving parking model on database!", e.Message));
            }
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]ParkingModel model)
        {

            if (model == null)
            {
                return BadRequest("Invalid ParkingModel parameter");
            }

            try
            {
                var parking = _mapper.Map<ParkingModel, Parking>(model);
                _mediaService.SaveParkingImage(model);
                _parkingRepository.Update(parking);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Parking, ParkingModel>(parking);
                return Ok(vm);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("{0} Exception: {1}", "Error updating parking model on database!", e.Message));
            }
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _parkingRepository.Find(id);
            if (entity == null)
                return BadRequest("Can't deleted, entity not found.");
            var vm = _mapper.Map<Parking, ParkingModel>(entity);
            _parkingRepository.Delete(entity);

            await _unitOfWork.CommitAsync();
            return Ok(vm);
        }

        [HttpGet("rentedbyhours")]
        public async Task<IActionResult> GetParkingRentedByHours([FromBody]int userId)
        {
            try
            {
                var orders = await _orderRepository.FindAllWhereAsync(o => o.UserId == userId && o.OrderStatus == OrderStatus.Actived);

                if (orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId, new CancellationToken());
                        var vm = _mapper.Map<Parking, ParkingModel>(parking);
                        return Ok(vm);
                    }
                }

                return NoContent();

            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }

        }

        [HttpPost("rentedbymonth")]
        public async Task<IActionResult> GetParkingRentedForMonth([FromBody]int userId)
        {
            var orders = await _orderRepository.FindAllWhereAsync(o =>
                o.UserId == userId && o.OrderStatus == OrderStatus.Actived);

            if (orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId, new CancellationToken());
                    var vm = _mapper.Map<Parking, ParkingModel>(parking);
                    return Ok(vm);
                }
            }

            return NoContent();
        }


    }
}