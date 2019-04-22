using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class OrderDataService : IOrderDataService
    {
        private readonly IApiService _apiService;

        public OrderDataService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<ApiResponse<OrderModel>> TerminateOrderAsync(int id)
        {
            try
            {
                var url = $"{ApiSettings.OrdersEndPoint}/terminate";
                var response = await _apiService.Post<int,OrderModel>(url, id).ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting model on server: {ex.Message}");
            }
        }
        public async Task<ApiResponse<OrderModel>> RenovateOrderAsync(int id,OrderModel order)
        {
            try
            {
                var url = $"{ApiSettings.OrdersEndPoint}/{id}/renew";
                var response = await _apiService.Put(url, order).ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting model on server: {ex.Message}");
            }
        }

        public async Task<ApiResponse<OrderModel>> CreateOrderAsync(OrderModel model)
        {
           
                var url = ApiSettings.OrdersEndPoint;
                var response = await _apiService.Post(url, model).ConfigureAwait(false);
                return response;
        }

        public async Task<OrderModel> GetOrderAsync(int orderId)
        {
            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Get<OrderModel>($"{url}/{orderId}").ConfigureAwait(false);

            return response.IsSuccess ? response.Result : null;
        }

        public async Task<List<OrderModel>> GetAllOrdersAsync()
        {
            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Get<List<OrderModel>> (url).ConfigureAwait(false);

            return response.IsSuccess ? response.Result : null;
        }

        public async Task<ApiResponse<OrderModel>> EditOrderAsync(int id, OrderModel order)
        {
            var url = ApiSettings.OrdersEndPoint;
            var response = await _apiService.Put(url, id, order).ConfigureAwait(false);
            return response;
        }

        public async Task<OrderModel> DeleteOrdersAsync(int id)
        {
            var url = ApiSettings.EventsEndPoint;
            var response = await _apiService.Delete<OrderModel>(url, id).ConfigureAwait(false);

            return response.IsSuccess ? response.Result : null;
        }
    }
}
