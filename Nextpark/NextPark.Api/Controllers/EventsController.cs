using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IRepository<Event> _repository;
        private readonly IRepository<Parking> _repositoryParking;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EventsController(IRepository<Event> repository, IRepository<Parking> repositoryParking, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _repositoryParking = repositoryParking;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // GET api/controller
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _repository.FindAllAsync();
            var vm = _mapper.Map<List<Event>, List<EventModel>>(list);
            return Ok(vm);
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _repository.Find(id);

            if (entity == null)
            {
                return BadRequest("Entity not found");
            }

            var vm = _mapper.Map<Event, EventModel>(entity);

            return Ok(vm);
        }

        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventModel model)
        {
            if (model == null)
            {
                return BadRequest("adding null entity");
            }

            var parking = _repositoryParking.Find(model.ParkingId);

            if (parking == null)
            {
                return BadRequest("Parking not found");
            }

            var eventsToCreate = CreateEvent(model, parking);

            if (eventsToCreate == null) {

                return BadRequest("Any event has been generated");
            }
            foreach (var ev in eventsToCreate) {
                _repository.Add(ev);
            }

            await _unitOfWork.CommitAsync();

            var vm = _mapper.Map<List<Event>, List<EventModel>>(eventsToCreate);

            return Ok(vm);
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]EventModel model)
        {
            if (model == null)
            {
                return BadRequest("model is null");
            }

            var entityEvent = _repository.Find(id);

            if (entityEvent == null)
            {
                return BadRequest("Event not found");
            }

            _repository.Update(entityEvent);

            var vm = _mapper.Map<Event, EventModel>(entityEvent);

            await _unitOfWork.CommitAsync();

            return Ok(vm);
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _repository.Find(id);

            if (entity == null)
            {
                return BadRequest("Can't deleted, entity not found.");
            }

            var vm = _mapper.Map<Event, EventModel>(entity);

            _repository.Delete(entity);

            await _unitOfWork.CommitAsync();

            return Ok(vm);
        }

        private List<Event> CreateEvent(EventModel model, Parking parking)
        {
            if (model == null || parking == null)
            {
                return null;
            }

            switch (model.RepetitionType)
            {
                case Enums.Enums.RepetitionType.Dayly:
                    return GenerateDaylyRepetition(model, parking);
                case Enums.Enums.RepetitionType.Weekly:
                    return null;
                case Enums.Enums.RepetitionType.Monthly:
                    return null;
                default:

                    return new List<Event>
                    {
                        new Event
                        {
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            RepetitionEndDate = model.EndDate,
                            RepetitionType = Enums.Enums.RepetitionType.None,
                            ParkingId = parking.Id
                        }
                    };
            }

        }
        private List<Event> GenerateDaylyRepetition(EventModel model, Parking parking)
        {
            var result = new List<Event>();

            var repetitionId = Guid.NewGuid();

            var currentDate = model.StartDate;
            var diffDays = (model.EndDate - currentDate).Days;

            while (diffDays >= 0)
            {

                var newEvent = new Event
                {
                    StartDate = currentDate,
                    EndDate = currentDate,
                    RepetitionEndDate = model.EndDate,
                    RepetitionType = Enums.Enums.RepetitionType.Dayly,
                    RepetitionId = repetitionId,
                    ParkingId = parking.Id
                };

                result.Add(newEvent);
                currentDate.AddDays(1);
                diffDays = (model.EndDate - currentDate).Days;
            }

            return result;
        }
    }
}