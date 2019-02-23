using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                return  NotFound("Parking not found");
            }

            var entity = _mapper.Map<EventModel, Event>(model);

            _repository.Add(entity);

            await _unitOfWork.CommitAsync();

            var vm = _mapper.Map<Event, EventModel>(entity);

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
                return NotFound("Event not found");
            }

            var updatedEntity = _mapper.Map<EventModel, Event>(model);

            _repository.Update(updatedEntity);

            await _unitOfWork.CommitAsync();

            var vm = _mapper.Map<Event, EventModel>(entityEvent);

            return Ok(vm);
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _repository.Find(id);

            if (entity == null)
            {
                return NotFound("Can't deleted, entity not found.");
            }

            var vm = _mapper.Map<Event, EventModel>(entity);

            _repository.Delete(entity);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            return Ok(vm);
        }

        private List<Event> CreateEvent(EventModel model)
        {
            switch (model.RepetitionType)
            {
                case Enums.Enums.RepetitionType.Dayly:
                    return GenerateDaylyRepetition(model);
                case Enums.Enums.RepetitionType.Weekly:
                    return GenerateWeeklyRepetition(model);
                case Enums.Enums.RepetitionType.Monthly:
                   return GenerateMonthlyRepetition(model);
                default:

                    return new List<Event>
                    {
                        new Event
                        {
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            RepetitionEndDate = model.EndDate,
                            RepetitionType = Enums.Enums.RepetitionType.None,
                            ParkingId = model.ParkingId
                        }
                    };
            }

        }
        private List<Event> GenerateDaylyRepetition(EventModel model)
        {
            var result = new List<Event>();

            var currentDate = model.StartDate;
            var diffDays = (model.EndDate - currentDate).Days;
            var repetitionId = Guid.NewGuid();

            while (diffDays >= 0)
            {

                var newEvent = new Event
                {
                    StartDate = currentDate,
                    EndDate = currentDate,
                    RepetitionEndDate = model.EndDate,
                    RepetitionId = repetitionId,
                    RepetitionType = Enums.Enums.RepetitionType.Dayly,
                ParkingId = model.ParkingId
                };

                result.Add(newEvent);
                currentDate.AddDays(1);
                diffDays = (model.EndDate - currentDate).Days;
            }

            return result;
        }

        private List<Event> GenerateWeeklyRepetition(EventModel model)
        {
            var result = new List<Event>();
            if (model.WeeklyRepeDayOfWeeks.Count>0)
            {
                var endDate = model.EndDate;
                var repetitionId = Guid.NewGuid();

                for (var date = model.StartDate; date <= model.EndDate; date = date.AddDays(1))
                {
                    var currentDayOfWeek = date.DayOfWeek;

                    if (model.WeeklyRepeDayOfWeeks.Any(dayRepeated => dayRepeated == currentDayOfWeek))
                    {
                        var newEvent = new Event
                        {
                            StartDate = date,
                            EndDate = new DateTime(date.Year, date.Month, date.Day, endDate.Hour, endDate.Minute, endDate.Second),
                            RepetitionEndDate = endDate,
                            RepetitionType = Enums.Enums.RepetitionType.Weekly,
                            RepetitionId = repetitionId,
                            ParkingId = model.ParkingId
                        };
                        result.Add(newEvent);
                    }
                }
            }

            return result;
        }

        private List<Event> GenerateMonthlyRepetition(EventModel model)
        {
            var result = new List<Event>();
            if (model.MonthlyRepeatDay.Count>0)
            {
                var endDate = model.EndDate;
                var startDate = model.StartDate;
                var repetitionId = Guid.NewGuid();

                foreach (var dayNumber in model.MonthlyRepeatDay)
                {
                    var newDate = new DateTime(startDate.Year,startDate.Month,dayNumber,startDate.Hour,startDate.Minute,startDate.Second);
                    for (var date  = newDate; date <= model.EndDate; date = date.AddMonths(1))
                    {
                        var newEvent = new Event
                        {
                            StartDate = date,
                            EndDate = new DateTime(date.Year, date.Month, date.Day, endDate.Hour, endDate.Minute, endDate.Second),
                            RepetitionEndDate = endDate,
                            RepetitionType = Enums.Enums.RepetitionType.Monthly,
                            RepetitionId = repetitionId,
                            ParkingId = model.ParkingId
                        };
                        result.Add(newEvent);
                    }
                }
            }
            return result.OrderBy(e=>e.StartDate).ToList();
        }
    }
}