using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.DataInterface
{
    public interface IEventDataService
    {
        Task<List<EventModel>> CreateEventAsync(EventModel model);             //Create event [Post]
        Task<EventModel> GetEventAsync(int eventId);                           //Get event by eventId [Get]
        Task<List<EventModel>> GetAllEventsAsync();                            //Get all events [Get]
        Task<EventModel> EditEventsAsync(int id, EventModel eventModel);       //Update event [Put]
        Task<EventModel> DeleteEventsAsync(int id);                            //Delete event [Delete]                                
    }
}