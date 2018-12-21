using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public class ParkingDataService : IParkingDataService
    {
        private readonly ApiService _apiService;

        public ParkingDataService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<ParkingModel>> Get()
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            try
            {
                var response = await _apiService.Get<ParkingModel>(ApiSettings.ParkingsEndPoint);

                if (response.IsSuccess) return response.Result as List<ParkingModel>;
                else return new List<ParkingModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error getting model from server: {0}", ex.Message));
            }
        }
        public async Task<ParkingModel> Get(int id)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            try
            {
                var response = await _apiService.Get<ParkingModel>(ApiSettings.ParkingsEndPoint, id);

                if (response.IsSuccess) return response.Result as ParkingModel;
                else return new ParkingModel();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error getting model from server: {0}", ex.Message));
            }
        }

        public async Task<ParkingModel> Post(ParkingModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            try
            {
                var response = await _apiService.Post<ParkingModel>(ApiSettings.ParkingsEndPoint, model);

                if (response.IsSuccess) return response.Result as ParkingModel;
                else return null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error setting model on server: {0}", ex.Message));
            }
        }

        public async Task<ParkingModel> Put(ParkingModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            try
            {
                var response = await _apiService.Put<ParkingModel>(ApiSettings.ParkingsEndPoint, model.Id, model);

                if (response.IsSuccess) return response.Result as ParkingModel;
                else return null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error updating model on server: {0}", ex.Message));
            }
        }

        public async Task<ParkingModel> Delete(int id)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            try
            {
                var response = await _apiService.Delete<ParkingModel>(ApiSettings.ParkingsEndPoint, id);

                if (response.IsSuccess) return response.Result as ParkingModel;
                else return new ParkingModel();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error deleting parking from server: {0}", ex.Message));
            }
        }

    }
}
