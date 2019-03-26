using NextPark.Mobile.Settings;
using NextPark.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services.Data
{
    public class EventDataService : IEventDataService
    {
        private readonly IApiService _apiService;

        public EventDataService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<EventModel>> CreateEventAsync(EventModel model)
        {
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Post<EventModel,List<EventModel>>(url, model).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }
        
        public async Task<EventModel> GetEventAsync(int eventId)
        {
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Get<EventModel>($"{url}/{eventId}").ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<EventModel>> GetAllEventsAsync()
        {
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Get<List<EventModel>>(url).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<EventModel> EditEventsAsync(int id, EventModel eventModel)
        {
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Put(url,id,eventModel).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<EventModel>> EditSerieEventsAsync(EventModel eventModel)
        {
            var url = $"{ApiSettings.EventsEndPoint}/{eventModel.Id}/serie";
            var response = await _apiService.Put<EventModel, List<EventModel>>(url, eventModel).ConfigureAwait(false);

            return response.IsSuccess ? response.Result : null;
        }

        public async Task<EventModel> DeleteEventsAsync(int id)
        {
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Delete<EventModel>(url,id).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<EventModel>> DeleteSerieEventsAsync(int id)
        {
            var url = $"{ApiSettings.EventsEndPoint}/{id}/serie";
            var response = await _apiService.Delete<List<EventModel>>(url).ConfigureAwait(false);

            return response.IsSuccess ? response.Result : null;
        }

    }
}
