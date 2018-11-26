using AutoMapper;
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
    public class ParkingsController : BaseController<Parking, ParkingModel>
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ParkingCategory> _parkingCategoryRepository;
        private readonly IRepository<ParkingType> _parkingTypeRepository;
        private readonly IRepository<Event> _parkingEventRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IMediaService _mediaService;

        public ParkingsController(IRepository<Parking> repository, IUnitOfWork unitOfWork, IMapper mapper,
           IRepository<Order> orderRepository, IRepository<ParkingCategory> parkingCategoryRepository,
           IRepository<Event> parkingEventRepository,
           IHostingEnvironment appEnvironment,
           IEmailSender emailSender,
           IRepository<ParkingType> parkingTypeRepository,
           IMediaService mediaService) : base(repository,
           unitOfWork, mapper)
        {
            _parkingRepository = repository;
            _orderRepository = orderRepository;
            _appEnvironment = appEnvironment;
            _parkingTypeRepository = parkingTypeRepository;
            _parkingCategoryRepository = parkingCategoryRepository;
            _parkingEventRepository = parkingEventRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _mediaService = mediaService;

        }

        // GET api/controller
        [HttpGet]
        protected override async Task<IActionResult> Get()
        {
            var parkigns = await _parkingRepository.FindAllAsync(new CancellationToken(), p => p.ParkingType,
                               p => p.ParkingCategory, p => p.ParkingEvent);

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

        // POST api/controller
        [HttpPost]
        protected virtual async Task<IActionResult> Post([FromBody] ParkingModel model)
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

                var category = _parkingCategoryRepository.Find(model.ParkingCategoryId);
                parking.ParkingCategory = category;

                var parkingEvent = _parkingEventRepository.Find(model.ParkingEventId);
                parking.ParkingEvent = parkingEvent;

                var parkingType = _parkingTypeRepository.Find(model.ParkingTypeId);
                parking.ParkingType = parkingType;
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
        protected virtual async Task<ActionResult> Put(int id, [FromBody]ParkingModel model)
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
                        var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId, new CancellationToken(),
                            p => p.ParkingType, p => p.ParkingCategory);

                        if (parking.ParkingType.Type == "By Hours")
                        {
                            var vm = _mapper.Map<Parking, ParkingModel>(parking);
                            return Ok(vm);
                        }
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
                    var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId, new CancellationToken(),
                        p => p.ParkingType, p => p.ParkingCategory);
                    if (parking.ParkingType.Type == "For Month")
                    {
                        var vm = _mapper.Map<Parking, ParkingModel>(parking);
                        return Ok(vm);
                    }
                }
            }

            return NoContent();
        }


    }
}