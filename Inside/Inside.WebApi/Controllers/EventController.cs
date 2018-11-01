using System.Threading.Tasks;
using AutoMapper;
using Inside.Data.Infrastructure;
using Inside.Data.Repositories;
using Inside.Domain.Entities;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Inside.WebApi.Controllers
{
    [Route("api/Event")]
    public class EventController : BaseController<Event, EventViewModel>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EventController(IRepository<Event> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository,
            unitOfWork, mapper)
        {
            _eventRepository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        [HttpPost("addevent")]
        public new async Task<IActionResult> Add([FromBody]EventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parkingEvent = _mapper.Map<EventViewModel, Event>(model);
            this._eventRepository.Add(parkingEvent);
            await _unitOfWork.CommitAsync();
            var vm = _mapper.Map<Event, EventViewModel>(parkingEvent);
            return Ok(vm);
        }
    }
}