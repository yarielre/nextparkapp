using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Enum;
using Inside.Domain.Models;
using Inside.Web.Helpers;
using Inside.Web.Infrastructure;
using Inside.Web.Repositories;
using Inside.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers.Api
{
    [Route("api/Parking")]
    public class ParkingController : BaseController<Parking, ParkingModel>
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ParkingCategory> _parkingCategoryRepository;
        private readonly IRepository<ParkingType> _parkingTypeRepository;
        private readonly IRepository<Event> _parkingEventRepository;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailSender _emailSender;


        public ParkingController(IRepository<Parking> repository, IUnitOfWork unitOfWork, IMapper mapper,
            IRepository<Order> orderRepository, IRepository<ParkingCategory> parkingCategoryRepository,
            IRepository<Event> parkingEventRepository, IHostingEnvironment appEnvironment, IEmailSender emailSender, IRepository<ParkingType> parkingTypeRepository) : base(repository,
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
            _emailSender = emailSender as EmailSender;

        }

        [HttpGet("getallparking")]
        public new async Task<IActionResult> GetAll()
        {
            var parkigns =
                await _parkingRepository.FindAllAsync(new CancellationToken(), p => p.ParkingType,
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

        [HttpPost("addparking")]
        public async Task<IActionResult> Add([FromBody]ParkingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                SaveParkingImage(model);
            }
            catch (Exception e)
            {
                //TODO: Remove and add logger
                _emailSender.SendDebugMessage("Parking", "Add", string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
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
                //TODO: Remove and add logger
                _emailSender.SendDebugMessage("Parking", "Add", string.Format("{0} Exception: {1}", "Error saving parking model on database!", e.Message));
                return BadRequest(string.Format("{0} Exception: {1}", "Error saving parking model on database!", e.Message));
            }

        }

        [HttpPost("editparking")]
        public async Task<IActionResult> Edit([FromBody]ParkingModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                SaveParkingImage(model);
            }
            catch (Exception e)
            {
                //TODO: Remove and add logger
                _emailSender.SendDebugMessage("Parking", "Edit", string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
                return BadRequest(string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
            }

            var parking = _mapper.Map<ParkingModel, Parking>(model);

            try
            {

                _parkingRepository.Update(parking);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                //TODO: Remove and add logger
                _emailSender.SendDebugMessage("Parking", "Edit", string.Format("{0} Exception: {1}", "Error updating parking model on database!", e.Message));
                return BadRequest(string.Format("{0} Exception: {1}", "Error updating parking model on database!", e.Message));
            }

            var vm = _mapper.Map<Parking, ParkingModel>(parking);

            return Ok(vm);
        }
        [HttpPost("parkingrentedbyhours")]
        public async Task<IActionResult> GetParkingRentedByHours([FromBody]int userId)
        {
            var orders = await _orderRepository.FindAllWhereAsync(o =>
                o.UserId == userId && o.OrderStatus == OrderStatus.Actived);
        
            if (orders.Count>0)
            {
                foreach (var order in orders)
                {
                    var parking =await _parkingRepository.FirstWhereAsync(p => p.Id == order.ParkingId, new CancellationToken(),
                        p => p.ParkingType,p=>p.ParkingCategory);
                    if (parking.ParkingType.Type == "By Hours")
                    {
                        var vm = _mapper.Map<Parking, ParkingModel>(parking);
                        return Ok(vm);
                    }
                }
            }

            return NoContent();
        }

        [HttpPost("parkingrentedformonth")]
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

        private void SaveParkingImage(ParkingModel model)
        {
            if (model.ImageBinary != null && model.ImageBinary.Length > 0)
            {
                var stream = new MemoryStream(model.ImageBinary);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var filePath = string.Format("/{0}/{1}", "Images", file);
                var folder = Path.Combine(_appEnvironment.WebRootPath, "Images");

                try
                {
                    var response = FilesHelper.UploadPhoto(stream, folder, file);
                    if (response)
                    {
                        model.ImageUrl = filePath;
                        return;
                    }
                }
                catch (Exception e) {
                    //TODO: Remove and add logger
                    _emailSender.SendDebugMessage("Parking", "Add/Edit", string.Format("Error generating imageUrl for parking {0} Exception: {1}", model.Id, e.Message));
                    return;
                }
                //TODO: Remove and add logger
                _emailSender.SendDebugMessage("Parking", "Add/Edit", string.Format("{0} : {1}", "Error generating imageUrl for parking", model.Id));
            }
        }
    }
}