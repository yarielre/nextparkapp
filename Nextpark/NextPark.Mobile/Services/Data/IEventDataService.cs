using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IEventDataService
    {
        Task<List<EventModel>> CreateEventAsync(EventModel model);
        Task<EventModel> DeleteEventsAsync(int id);
        Task<EventModel> EditEventsAsync(int id, EventModel eventModel);
        Task<List<EventModel>> GetAllEventsAsync();
        Task<EventModel> GetEventAsync(int eventId);
    }
}