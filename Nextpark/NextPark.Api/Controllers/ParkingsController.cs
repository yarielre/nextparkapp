using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Enums.Enums;
using NextPark.Models;
using NextPark.Services;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class ParkingsController : ControllerBase
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IRepository<Order> _orderRepository;

        private readonly IRepository<Event> _parkingEventRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IUnitOfWork _unitOfWork;

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
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var parkigns = await _parkingRepository.FindAllAsync().ConfigureAwait(false);
            var vm = _mapper.Map<List<Parking>, List<ParkingModel>>(parkigns);
            return Ok(ApiResponse<List<ParkingModel>>.GetSuccessResponse(vm));
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _parkingRepository.Find(id);
            if (entity == null)
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse("Entity not found",ErrorType.EntityNotFound));
            var vm = _mapper.Map<Parking, ParkingModel>(entity);
            return Ok(ApiResponse<ParkingModel>.GetSuccessResponse(vm));
        }

        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ParkingModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse("Model is null",ErrorType.EntityNull));

            try
            {
                var imageUrl = _mediaService.SaveImage(model.ImageBinary);
                if (!string.IsNullOrEmpty(imageUrl)) model.ImageUrl = imageUrl;
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse(e.Message, ErrorType.Exeption));
                //Log: return BadRequest(string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
            }

            try
            {
                var parking = _mapper.Map<ParkingModel, Parking>(model);
                _parkingRepository.Add(parking);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Parking, ParkingModel>(parking);
                return Ok(ApiResponse<ParkingModel>.GetSuccessResponse(vm));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse(e.Message, ErrorType.Exeption));
            }
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ParkingModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse("Model is null",ErrorType.EntityNull));

            try
            {
                var parking = _mapper.Map<ParkingModel, Parking>(model);
                try
                {
                    var imageUrl = _mediaService.SaveImage(model.ImageBinary);
                    if (!string.IsNullOrEmpty(imageUrl))
                        model.ImageUrl = imageUrl;
                }
                catch (Exception e)
                {
                    return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse(e.Message, ErrorType.Exeption));
                    //Log: return BadRequest(string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
                }

                _parkingRepository.Update(parking);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Parking, ParkingModel>(parking);
                return Ok(ApiResponse<ParkingModel>.GetSuccessResponse(vm));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse(e.Message,ErrorType.Exeption));
            }
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _parkingRepository.Find(id);
            if (entity == null)
                return BadRequest(ApiResponse<ParkingModel>.GetErrorResponse("Can't deleted, entity not found.",ErrorType.EntityNotFound));
            var vm = _mapper.Map<Parking, ParkingModel>(entity);
            _parkingRepository.Delete(entity);

            await _unitOfWork.CommitAsync();
            return Ok(ApiResponse<ParkingModel>.GetSuccessResponse(vm));
        }

        [HttpGet("{id}/events")]
        public async Task<IActionResult> GetEvents(int id)
        {
            var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == id
                                                                  && p.Events.Any(e => e.StartDate >= DateTime.Now), 
                                                                  new CancellationToken(),
                                                                  p => p.Events).ConfigureAwait(false);
            if (parking == null)
            {
                return BadRequest(ApiResponse<List<EventModel>>.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));
            }

            if (parking.Events == null && parking.Events.Count == 0) {
                return BadRequest(ApiResponse<List<EventModel>>.GetErrorResponse("Parking don't have events",ErrorType.EntityNotFound));
            }
            var vm = _mapper.Map<List<Event>, List<EventModel>>(parking.Events);
            return Ok(ApiResponse<List<EventModel>>.GetSuccessResponse(vm));
        }

        [HttpDelete("{id}/events")]
        public async Task<IActionResult> DeleteEvents(int id)
        {
            var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == id, new CancellationToken(),
                p => p.Events).ConfigureAwait(false);

            if (parking == null) return BadRequest(ApiResponse<List<EventModel>>.GetErrorResponse("Parking not found",ErrorType.EntityNotFound));

            if (parking.Events == null && parking.Events.Count == 0)
            {
                return BadRequest(ApiResponse<List<EventModel>>.GetErrorResponse("Parking don't have events", ErrorType.EntityNotFound));
            }
            var vm = _mapper.Map<List<Event>, List<EventModel>>(parking.Events);
            foreach (var ev in parking.Events) {
                _parkingEventRepository.Delete(ev);
            }
            await _unitOfWork.CommitAsync().ConfigureAwait(false);
            return Ok(ApiResponse<List<EventModel>>.GetSuccessResponse(vm));
        }

        [HttpPost("rentedbyuserdayly")]
        public async Task<IActionResult> GetParkingRentedByUserDayly([FromBody] int userId)
        {
            //Need to know the logic of this method 22/03/2019 i think is wrong
            try
            {
                var orders =
                    await _orderRepository.FindAllWhereAsync(o =>
                        o.UserId == userId && o.OrderStatus == OrderStatus.Actived).ConfigureAwait(false);

                if (orders.Count > 0)
                    foreach (var order in orders)
                    {
                        var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId
                                                                                    && p.Events.Any(e => e.RepetitionType == RepetitionType.Dayly),
                                                                                    new CancellationToken(),
                                                                                    p=>p.Events).ConfigureAwait(false);
                        var vm = _mapper.Map<Parking, ParkingModel>(parking);
                        return Ok(vm);
                    }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }

        [HttpPost("rentedbyuserweekly")]
        public async Task<IActionResult> GetParkingRentedByUserWeekly([FromBody] int userId)
        {
            //Need to know the logic of this method 22/03/2019 beacuse i think is wrong
            try
            {
                var orders =
                    await _orderRepository.FindAllWhereAsync(o =>
                        o.UserId == userId && o.OrderStatus == OrderStatus.Actived);

                if (orders.Count > 0)
                    foreach (var order in orders)
                    {
                        var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId
                                                                                    && p.Events.Any(e => e.RepetitionType == RepetitionType.Weekly), new CancellationToken(),
                            p => p.Events);

                        var vm = _mapper.Map<Parking, ParkingModel>(parking);
                        return Ok(vm);
                    }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }

        [HttpPost("rentedbyusermonthtly")]
        public async Task<IActionResult> GetParkingRentedByUserMonthly([FromBody] int userId)
        {
            //Need to know the logic of this method 22/03/2019 beacuse i think is wrong
            try
            {
                var orders =
                    await _orderRepository.FindAllWhereAsync(o =>
                        o.UserId == userId && o.OrderStatus == OrderStatus.Actived);

                if (orders.Count > 0)
                    foreach (var order in orders)
                    {
                        var parking = await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId
                                                                                    && p.Events.Any(e => e.RepetitionType == RepetitionType.Monthly), new CancellationToken(),
                            p => p.Events);

                        var vm = _mapper.Map<Parking, ParkingModel>(parking);
                        return Ok(vm);
                    }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }
    }
}