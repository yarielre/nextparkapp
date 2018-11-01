using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Data.Infrastructure;
using Inside.Data.Repositories;
using Inside.Domain.Entities;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inside.WebApi.Controllers
{
    [Route("api/Parking")]
    public class ParkingController : BaseController<Parking, ParkingViewModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ParkingCategory> _parkingCategoryRepository;
        private readonly IRepository<Event> _parkingEventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _appEnvironment;


        public ParkingController(IRepository<Parking> repository, IUnitOfWork unitOfWork, IMapper mapper,
            IRepository<Order> orderRepository, IRepository<ParkingCategory> parkingCategoryRepository, IRepository<Event> parkingEventRepository, IHostingEnvironment appEnvironment) : base(repository, unitOfWork, mapper)
        {
            _parkingRepository = repository;
            _orderRepository = orderRepository;
            _appEnvironment = appEnvironment;
            _parkingCategoryRepository = parkingCategoryRepository;
            _parkingEventRepository = parkingEventRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("getallparking")]
        public new async Task<IActionResult> GetAll()
        {
            List<Parking> parkigns =
                await _parkingRepository.FindAllAsync(new System.Threading.CancellationToken(), p => p.ParkingType,p=>p.ParkingCategory,p=>p.ParkingEvent);
            if (parkigns.Count>0) 
            {
                foreach (Parking park in parkigns)
                {
                    if (park.IsRented)
                    {
                        Order rent = await _orderRepository.FirstOrDefaultWhereAsync(o =>
                            o.ParkingId == park.Id && o.OrderStatus == Domain.Enum.OrderStatus.Actived);
                    }
                }
            
            }
            var vm = _mapper.Map<List<Parking>, List<ParkingViewModel>>(parkigns);
            return Ok(vm);
        }

        [HttpPost("addparking")]
        public new async Task<IActionResult> Add(Parking parking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                var files = HttpContext.Request.Form.Files;
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Images");
            if (files.Count>0)
            {
                var parkingImage = files[0];
                if (parkingImage != null && parkingImage.Length > 0)
                {
                    
                    var fileName = Guid.NewGuid().ToString().Replace("-", "") +
                                   Path.GetExtension(parkingImage.FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        await parkingImage.CopyToAsync(fileStream);
                        parking.ImageUrl = fileName;
                    }
                }
            }
            else
            {
                parking.ImageUrl = "parking.jpg";
            }
            this._parkingRepository.Add(parking);
            await  this._unitOfWork.CommitAsync();
            var category = _parkingCategoryRepository.Find(parking.ParkingCategoryId);
            parking.ParkingCategory = category;
            var parkingEvent = this._parkingEventRepository.Find(parking.ParkingEventId);
            parking.ParkingEvent = parkingEvent;
            var vm = _mapper.Map<Parking, ParkingViewModel>(parking);
            return Ok(vm);
        }

        [HttpPost("editparking")]
        public async Task<IActionResult> EditParking(ParkingViewModel parkingToEdit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var parking = _mapper.Map<ParkingViewModel, Parking>(parkingToEdit);
            _parkingRepository.Update(parking);
            
            await _unitOfWork.CommitAsync();
            var vm = _mapper.Map<Parking, ParkingViewModel>(parking);
            return Ok(vm);
        }
    }
}