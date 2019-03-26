using System;
using System.Threading.Tasks;
using NextPark.Mobile.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class PurchaseDataService : IPurchaseDataService
    {
        private readonly IApiService _apiService;

        public PurchaseDataService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<PurchaseModel> BuyAmount(PurchaseModel model)
        {
                var url = $"{ApiSettings.PurchaseEndPoint}/amount" ;

                var response = await _apiService.Post<PurchaseModel, PurchaseModel>(url, model).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }

        public async Task<PurchaseModel> DrawalCash(PurchaseModel model)
        {
            
                var url = $"{ApiSettings.PurchaseEndPoint}/drawal";

                var response = await _apiService.Post<PurchaseModel, PurchaseModel>(url, model).ConfigureAwait(false);

                return response.IsSuccess ? response.Result : null;
        }
    }
}
