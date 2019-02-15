using NextPark.Mobile.Core.Settings;
using NextPark.Models;
using System;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services.Data
{
    public class OrderDataService : BaseDataService<OrderModel>
    {
        public OrderDataService(IApiService apiService) : base(apiService, ApiSettings.OrdersEndPoint)
        {
        }
        public async Task<ParkingModel> TerminateOrder(int id)
        {
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
               var url = $"{ApiSettings.OrdersEndPoint}/terminate";
                var response = await ApiService.Post(url, id);

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
            var isConneted = await ApiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.OrdersEndPoint}/renew";
                var response = await ApiService.Post<RenovateOrder>(url, model);

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
    }
}
