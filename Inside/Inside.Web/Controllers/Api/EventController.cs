using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using Inside.Web.Infrastructure;
using Inside.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers.Api
{
    [Route("api/Event")]
    public class EventController : BaseController<Event, EventModel>
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
        public async Task<IActionResult> Add([FromBody]EventModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parkingEvent = _mapper.Map<EventModel, Event>(model);
            this._eventRepository.Add(parkingEvent);
            await _unitOfWork.CommitAsync();
            var vm = _mapper.Map<Event, EventModel>(parkingEvent);
            return Ok(vm);
        }
    }
}