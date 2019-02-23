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
    public class ProfileService
    {
        private readonly ApiService _apiService;

        public ProfileService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> ChangePassword(ChangePasswordModel model)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.ProfileEndPoint}/editpass";
                var response = await _apiService.Post(url, model);

                if (response.IsSuccess)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating event on server: {ex.Message}");
            }
        }

        public async Task<UserModel> EditProfile(EditProfileModel model)
        {
            var isConneted = await _apiService.CheckConnection().ConfigureAwait(false);
            if (!isConneted.IsSuccess)
            {
                throw new Exception("Internet correction error.");
            }

            try
            {
                var url = $"{ApiSettings.ProfileEndPoint}/edit";
                var response = await _apiService.Post<EditProfileModel, UserModel>(url, model);

                if (response.IsSuccess)
                {
                    return response.Result as UserModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating event on server: {ex.Message}");
            }
        }


        public async Task<EditProfileModel> UpdateUserData(EditProfileModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{ApiSettings.ProfileEndPoint}/edit";

            var client = _apiService.GetHttpClient();
            var response = await client.PostAsync(endpoint, content);

            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(string.Format("Bad request: {0}", response.ReasonPhrase));

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<EditProfileModel>(resultJson);

            return result == null ? new EditProfileModel() : result as EditProfileModel;
        }
    }
}
