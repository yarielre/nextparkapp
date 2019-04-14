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
using NextPark.Enums;
using NextPark.Enums.Enums;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IRepository<Event> _repository;
        private readonly IRepository<Parking> _repositoryParking;
        private readonly IRepository<Order> _repositoryOrder;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EventsController(IRepository<Event> repository, IRepository<Parking> repositoryParking, IUnitOfWork unitOfWork, IMapper mapper, IRepository<Order> repositoryOrder)
        {
            _repository = repository;
            _repositoryParking = repositoryParking;
            _mapper = mapper;
            _repositoryOrder = repositoryOrder;
            _unitOfWork = unitOfWork;
        }

        // GET api/controller
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _repository.FindAllAsync().ConfigureAwait(false);
            var vm = _mapper.Map<List<Event>, List<EventModel>>(list);
            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _repository.Find(id);

            if (entity == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Entity not found",ErrorType.EntityNotFound));
            }

            var vm = _mapper.Map<Event, EventModel>(entity);

            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventModel model)
        {
            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Adding null entity", ErrorType.EntityNull));
            }

            var parking = _repositoryParking.Find(model.ParkingId);

            if (parking == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Parking not found",ErrorType.EntityNotFound));
            }

            var events = CreateEvent(model);


            foreach (var ev in events)
            {
                _repository.Add(ev);
            }

            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            var vm = _mapper.Map<List<Event>, List<EventModel>>(events);

            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]EventModel model)
        {       
            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Editing null entity",ErrorType.EntityNull));
            }

            try // No allow modify an event who has an order pending or active associated to the event
            {
                var actualEvent = _repository.Find(id);
                if (actualEvent == null) {
                    return BadRequest(ApiResponse.GetErrorResponse("Event not found", ErrorType.EntityNotFound));
                }

                var updatedEvent = _mapper.Map<EventModel, Event>(model);
                
                // Get parking associated to the event
                var parking = await _repositoryParking.FirstOrDefaultWhereAsync(p => p.Id == actualEvent.ParkingId).ConfigureAwait(false);
                if (parking == null)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));
                }

                // Check if th event can be modified
                var eventCanBeModified = await EventCanBeModified(actualEvent, updatedEvent, parking).ConfigureAwait(false);
                if (!eventCanBeModified)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Event has an active order and can't be modified", ErrorType.EventCantBeModified));
                }

                _repository.Update(updatedEvent);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Event, EventModel>(updatedEvent);
                return Ok(ApiResponse.GetSuccessResponse(vm,"Event Updated"));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse(e.Message, ErrorType.Exception));
            }
        }

        [HttpPut("{id}/serie")]
        public async Task<ActionResult> PutSerie(int id, [FromBody]EventModel model)
        {
            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("model is null",ErrorType.EntityNull));
            }

            var entityEvent = _repository.Find(id);

            if (entityEvent == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Event not found",ErrorType.EntityNotFound));
            }

            var updatedEvent = _mapper.Map<EventModel, Event>(model);

            // Get parking associated to the event
            var parking = await _repositoryParking.FirstOrDefaultWhereAsync(p => p.Id == entityEvent.ParkingId).ConfigureAwait(false);
            if (parking == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));
            }

            // Check if all events of the serie can be modified
            var eventSerie = await _repository.FindAllWhereAsync(ev => ev.RepetitionId == entityEvent.RepetitionId).ConfigureAwait(false);
            var updatedSerie = new List<Event>();

            foreach (Event ev in eventSerie)
            {
                Event actualUpdatedEvent = ev;
                actualUpdatedEvent.StartDate = ev.StartDate + updatedEvent.StartDate.TimeOfDay;
                actualUpdatedEvent.EndDate = ev.EndDate + updatedEvent.EndDate.TimeOfDay;
                actualUpdatedEvent.RepetitionEndDate = updatedEvent.RepetitionEndDate;

                var eventCanBeModified = await EventCanBeModified(ev, actualUpdatedEvent, parking).ConfigureAwait(false);
                if (!eventCanBeModified)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Event has an active order and can't be modified", ErrorType.EventCantBeModified));
                }
                _repository.Update(actualUpdatedEvent);
                updatedSerie.Add(actualUpdatedEvent);
            }

            await _unitOfWork.CommitAsync().ConfigureAwait(false);
            
            var vm = _mapper.Map<List<Event>, List<EventModel>>(updatedSerie);

            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _repository.Find(id);

            if (entity == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Can't delete, entity not found.",ErrorType.EntityNotFound));
            }

            // Get parking associated to the event
            var parking = await _repositoryParking.FirstOrDefaultWhereAsync(p => p.Id == entity.ParkingId).ConfigureAwait(false);
            if (parking == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));
            }

            // Check if event can be deleted
            var eventCanBeDeleted = await EventCanBeDeleted(entity, parking).ConfigureAwait(false);
            if (!eventCanBeDeleted)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Event has an active order and can't be modified", ErrorType.EventCantBeModified));
            }

            var vm = _mapper.Map<Event, EventModel>(entity);
            _repository.Delete(entity);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            return Ok(ApiResponse.GetSuccessResponse(vm));
        }

        [HttpDelete("{id}/serie")]
        public async Task<IActionResult> DeleteSerie(int id)
        {
            var entity = _repository.Find(id);

            if (entity == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Can't deleted, entity not found.",ErrorType.EntityNotFound));
            }

            // Get parking associated to the event
            var parking = await _repositoryParking.FirstOrDefaultWhereAsync(p => p.Id == entity.ParkingId).ConfigureAwait(false);
            if (parking == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Parking not found", ErrorType.EntityNotFound));
            }

            // Check if all events of the serie can be deleted
            var eventSerie = await _repository.FindAllWhereAsync(ev => ev.RepetitionId == entity.RepetitionId);

            foreach (Event entityEvent in eventSerie) {
                var eventCanBeModified = await EventCanBeDeleted(entityEvent, parking).ConfigureAwait(false);
                if (!eventCanBeModified)
                {
                    return BadRequest(ApiResponse.GetErrorResponse("Event has an active order and can't be modified", ErrorType.EventCantBeModified));
                }
            }

            _repository.Delete(eventSerie);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            var vm = _mapper.Map<List<Event>, List<EventModel>>(eventSerie);

            return Ok(ApiResponse.GetSuccessResponse(vm));
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
                            RepetitionId = Guid.NewGuid(),
                            RepetitionType = RepetitionType.None,
                            ParkingId = model.ParkingId
                        }
                    };
            }

        }

        private List<Event> GenerateDaylyRepetition(EventModel model)
        {
            var result = new List<Event>();

            var currentDate = model.StartDate;
            
            var diffDays = (model.RepetitionEndDate - model.StartDate).TotalDays;

            var repetitionId = Guid.NewGuid();

            while (diffDays >= 0)
            {

                var newEvent = new Event
                {
                    StartDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, model.StartDate.Hour, model.StartDate.Minute, model.StartDate.Minute),
                    EndDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, model.EndDate.Hour, model.EndDate.Minute, model.EndDate.Minute),
                    RepetitionEndDate = model.RepetitionEndDate,
                    RepetitionId = repetitionId,
                    RepetitionType = Enums.Enums.RepetitionType.Dayly,
                    ParkingId = model.ParkingId
                };

                result.Add(newEvent);
                currentDate = currentDate.AddDays(1);
                diffDays = (model.RepetitionEndDate - currentDate).TotalDays;
            }

            return result;
        }

        private List<Event> GenerateWeeklyRepetition(EventModel model)
        {
            var result = new List<Event>();
            if (model.WeeklyRepeDayOfWeeks.Count > 0)
            {
                var endDate = model.RepetitionEndDate;
                var repetitionId = Guid.NewGuid();

                for (var date = model.StartDate; date <= model.RepetitionEndDate; date = date.AddDays(1))
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
            if (model.MonthlyRepeatDay.Count > 0)
            {
                var endDate = model.EndDate;
                var startDate = model.StartDate;
                var repetitionId = Guid.NewGuid();

                foreach (var dayNumber in model.MonthlyRepeatDay)
                {
                    var newDate = new DateTime(startDate.Year, startDate.Month, dayNumber, startDate.Hour, startDate.Minute, startDate.Second);
                    for (var date = newDate; date <= model.EndDate; date = date.AddMonths(1))
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
            return result.OrderBy(e => e.StartDate).ToList();
        }
        private async Task<bool> EventCanBeDeleted(Event eventToBeDeleted, Parking parking)
        {
            // Get orders associated to the parking with time match with the event
            var orders = await _repositoryOrder.FindAllWhereAsync(o => o.ParkingId == parking.Id
                                                                  && o.StartDate < eventToBeDeleted.EndDate
                                                                  && o.EndDate > eventToBeDeleted.StartDate).ConfigureAwait(false);

            // If not found orders, then event can be deleted
            return orders.Count == 0;
        }

        private async Task<bool> EventCanBeModified(Event actualEvent, Event updatedEvent, Parking parking)
        {
            // Get orders associated to the parking with time match with the event
            var orders = await _repositoryOrder.FindAllWhereAsync(o => o.ParkingId == parking.Id
                                                                  && o.StartDate < actualEvent.EndDate
                                                                  && o.EndDate > actualEvent.StartDate).ConfigureAwait(false);

            // Check if the change does affect an order
            foreach (Order order in orders)
            {
                if (((updatedEvent.StartDate.TimeOfDay > TimeSpan.FromMinutes(0)) && (updatedEvent.StartDate > order.StartDate)) || ((updatedEvent.EndDate.TimeOfDay < TimeSpan.FromMinutes(1439)) && (updatedEvent.EndDate < order.EndDate)))
                {
                    return false;
                }
            }

            // If no order is affected, then event can be modified
            return true;
        }
    }
}