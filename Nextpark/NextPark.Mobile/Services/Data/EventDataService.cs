using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.Settings;
using NextPark.Models;

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
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Post<EventModel,List<EventModel>>(url, model).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as List<EventModel>;
                }
                return null;
            }
        
        public async Task<EventModel> GetEventAsync(int eventId)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Get<EventModel>($"{url}/{eventId}").ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as EventModel;
                }
                return null;
        }

        public async Task<List<EventModel>> GetAllEventsAsync()
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Get<EventModel>(url).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as List<EventModel>;
                }
                return null;
        }

        public async Task<EventModel> EditEventsAsync(int id, EventModel eventModel)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Put(url,id,eventModel).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as EventModel;
                }
                return null;
        }

        public async Task<List<EventModel>> EditSerieEventsAsync(EventModel eventModel)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
            var url = $"{ApiSettings.EventsEndPoint}/{eventModel.Id}/serie";
            var response = await _apiService.Put(url, eventModel).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as List<EventModel>;
            }
            return null;
        }

        public async Task<EventModel> DeleteEventsAsync(int id)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
                var url = ApiSettings.EventsEndPoint;
                var response = await _apiService.Delete<EventModel>(url,id).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as EventModel;
                }
                return null;
        }

        public async Task<List<EventModel>> DeleteSerieEventsAsync(int id)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
            var url = $"{ApiSettings.EventsEndPoint}/{id}/serie";
            var response = await _apiService.Delete<EventModel>(url).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as List<EventModel>;
            }
            return null;
        }

    }
}
