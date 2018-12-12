using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApiService _apiService;

        public ProfileService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{ApiSettings.ProfileEndPoint}/editcoins";

            var client = _apiService.GetHttpClient();
            var response = await client.PostAsync(endpoint, content);

            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(string.Format("Bad request: {0}", response.ReasonPhrase) );

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UpdateUserCoinModel>(resultJson);

            return result == null ? new UpdateUserCoinModel() : result as UpdateUserCoinModel;
        }
    }
}
