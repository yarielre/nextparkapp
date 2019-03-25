using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Mobile.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class ParkingDataService : IParkingDataService
    {
        private readonly IApiService _apiService;

        public ParkingDataService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ParkingModel> CreateParkingAsync(ParkingModel model)
        {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Post(url, model).ConfigureAwait(false);
                return response.IsSuccess ? response.Result : null;
        }

        public async Task<ParkingModel> GetParkingAsync(int parkingId)
        {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Get<ParkingModel>($"{url}/{parkingId}").ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<ParkingModel>> GetAllParkingsAsync()
        {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Get<List<ParkingModel>>(url).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<ParkingModel> EditParkingAsync(ParkingModel parkingModel)
        {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Put(url, parkingModel.Id,  parkingModel).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
           
        }

        public async Task<ParkingModel> DeleteParkingsAsync(int id)
        {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Delete<ParkingModel>(url, id).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<EventModel>> DeleteParkingsEventsAsync(int id)
        {
           
                var url = $"{ApiSettings.ParkingsEndPoint}/{id}/events";
                var response = await _apiService.Delete<List<EventModel>>(url).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<EventModel>> GetParkingEventsAsync(int parkingId)
        {
                var url = $"{ApiSettings.ParkingsEndPoint}/{parkingId}/events";

                var response = await _apiService.Get<List<EventModel>>(url).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }
    }
}
