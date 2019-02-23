
using NextPark.Models;
using System;
using System.Threading.Tasks;
using NextPark.Mobile.Settings;

namespace NextPark.Mobile.Services.Data
{
    public class PurchaseDataService
    {
        private readonly IApiService _apiService;

        public PurchaseDataService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<PurchaseModel> BuyAmount(PurchaseModel model)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.PurchaseEndPoint}/amount" ;

                var response = await _apiService.Post<PurchaseModel, PurchaseModel>(url, model);

                if (response.IsSuccess)
                {
                    return response.Result as PurchaseModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating event on server: {ex.Message}");
            }
        }

        public async Task<PurchaseModel> DrawalCash(PurchaseModel model)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.PurchaseEndPoint}/drawal";

                var response = await _apiService.Post<PurchaseModel, PurchaseModel>(url, model);

                if (response.IsSuccess)
                {
                    return response.Result as PurchaseModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating event on server: {ex.Message}");
            }
        }
    }
}
