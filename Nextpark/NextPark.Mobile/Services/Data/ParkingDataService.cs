using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Post(url, model).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as ParkingModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating event on server: {ex.Message}");
            }
        }

        public async Task<ParkingModel> GetParkingAsync(int parkingId)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Get<ParkingModel>($"{url}/{parkingId}").ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as ParkingModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error geting parking on server: {ex.Message}");
            }
        }

        public async Task<List<ParkingModel>> GetAllParkingsAsync()
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Get<ParkingModel>(url).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as List<ParkingModel>;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error get all parkings on server: {ex.Message}");
            }
        }

        public async Task<ParkingModel> EditParkingAsync(ParkingModel parkingModel)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Put(url, parkingModel.Id,  parkingModel).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as ParkingModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error editing parking on server: {ex.Message}");
            }
        }

        public async Task<ParkingModel> DeleteParkingsAsync(int id)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = ApiSettings.ParkingsEndPoint;
                var response = await _apiService.Delete<ParkingModel>(url, id).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as ParkingModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting parking on server: {ex.Message}");
            }
        }

        public async Task<List<EventModel>> DeleteParkingsEventsAsync(int id)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.ParkingsEndPoint}/{id}/events";
                var response = await _apiService.Delete<EventModel>(url).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as List<EventModel>;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting parking on server: {ex.Message}");
            }
        }

        public async Task<List<EventModel>> GetParkingEventsAsync(int parkingId)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.ParkingsEndPoint}/{parkingId}/events";

                var response = await _apiService.Get<EventModel>(url).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as List<EventModel>;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error geting parking on server: {ex.Message}");
            }
        }
    }
}
