using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IEventDataService
    {
        Task<List<EventModel>> CreateEventAsync(EventModel model);
        Task<ApiResponse<EventModel>> DeleteEventsAsync(int id);
        Task<ApiResponse<List<EventModel>>> DeleteSerieEventsAsync(int id);
        Task<ApiResponse<EventModel>> EditEventsAsync(int id, EventModel eventModel);
        Task<ApiResponse<List<EventModel>>> EditSerieEventsAsync(EventModel eventModel);
        Task<List<EventModel>> GetAllEventsAsync();
        Task<EventModel> GetEventAsync(int eventId);
    }
}