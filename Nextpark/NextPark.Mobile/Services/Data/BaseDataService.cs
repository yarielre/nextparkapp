using NextPark.Mobile.Core.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services.Data
{
    public class BaseDataService<TModel> where TModel : class, new()
    {
        public ApiService ApiService { get; }

        private readonly string _apiEndPoint;

        public BaseDataService(ApiService apiService, string apiEndpoint)
        {
            ApiService = apiService;
            _apiEndPoint = apiEndpoint;
        }

        public async Task<List<TModel>> Get()
        {
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var response = await ApiService.Get<TModel>(_apiEndPoint);

                if (response.IsSuccess)
                {
                    return response.Result as List<TModel>;
                }
                else
                {
                    return new List<TModel>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error getting model from server: {0}", ex.Message));
            }
        }
        public async Task<TModel> Get(int id)
        {
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var response = await ApiService.Get<TModel>(_apiEndPoint, id);

                if (response.IsSuccess)
                {
                    return response.Result as TModel;
                }
                else
                {
                    return new TModel();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error getting model from server: {0}", ex.Message));
            }
        }
        public async Task<TModel> Post(TModel model)
        {
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var response = await ApiService.Post<TModel>(_apiEndPoint, model);

                if (response.IsSuccess)
                {
                    return response.Result as TModel;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error setting model on server: {0}", ex.Message));
            }
        }
        public async Task<TModel> Put(TModel model, int id)
        {
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var response = await ApiService.Put<TModel>(_apiEndPoint, id, model);

                if (response.IsSuccess)
                {
                    return response.Result as TModel;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error updating model on server: {0}", ex.Message));
            }
        }
        public async Task<TModel> Delete(int id)
        {
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var response = await ApiService.Delete<TModel>(_apiEndPoint, id);

                if (response.IsSuccess)
                {
                    return response.Result as TModel;
                }
                else
                {
                    return new TModel();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error deleting model from server: {0}", ex.Message));
            }
        }
    }
}
