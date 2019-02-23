using NextPark.Mobile.Settings;
using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.Services.DataInterface;

namespace NextPark.Mobile.Services.Data
{
    public class OrderDataService : IOrderDataService
    {
        private readonly IApiService _apiService;

        public OrderDataService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<ParkingModel> TerminateOrder(int id)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
               var url = $"{ApiSettings.OrdersEndPoint}/terminate";
                var response = await _apiService.Post(url, id).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as ParkingModel;
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
        public async Task<OrderModel> RenovateOrder(RenovateOrder model)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.OrdersEndPoint}/renew";
                var response = await _apiService.Post(url, model).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    return response.Result as OrderModel;
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

        public async Task<OrderModel> CreateOrderAsync(OrderModel model)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Post(url, model).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as OrderModel;
            }
            return null;
        }

        public async Task<OrderModel> GetOrderAsync(int orderId)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Get<OrderModel>($"{url}/{orderId}").ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as OrderModel;
            }
            return null;
        }

        public async Task<List<OrderModel>> GetAllOrdersAsync()
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Get<OrderModel>(url).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as List<OrderModel>;
            }
            return null;
        }

        public async Task<OrderModel> EditOrderAsync(int id, OrderModel order)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Put(url, id, order).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as OrderModel;
            }
            return null;
        }

        public async Task<OrderModel> DeleteOrdersAsync(int id)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }
            var url = ApiSettings.EventsEndPoint;
            var response = await _apiService.Delete<OrderModel>(url, id).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return response.Result as OrderModel;
            }
            return null;
        }
    }
}
