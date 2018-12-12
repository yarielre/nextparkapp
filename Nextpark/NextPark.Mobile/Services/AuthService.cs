using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public class AuthService : IAuthService
    {

        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }
        public bool IsUserAuthenticated() {
            return false;
        }
        public async Task<TokenResponse> Login(string endpoint, string username, string password)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return new TokenResponse { IsSuccess = isConneted.IsSuccess };

            try
            {
                var model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{endpoint}/login";

                var http = _apiService.GetHttpClient();
                var response = await http.PostAsync(endpoint, content);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJson);
                result.IsSuccess = true;
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TokenResponse> Logout(string endpoint)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return new TokenResponse { IsSuccess = isConneted.IsSuccess };

            try
            {

                var url = $"{endpoint}/logout";

                var http = _apiService.GetHttpClient();
                var response = await http.GetAsync(endpoint);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJson);
                result.IsSuccess = true;

                return result;

            }
            catch
            {
                return null;
            }
        }

        public async Task<TokenResponse> Register(string endpoint, RegisterModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return new TokenResponse { IsSuccess = isConneted.IsSuccess };

            try
            {

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{endpoint}/register";

                var http = _apiService.GetHttpClient();
                var response = await http.PostAsync(endpoint, content);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJson);
                result.IsSuccess = true;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public async Task<Response> GetUserByUserName(string endpoint, string userName)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{endpoint}/userbyname";
                var json = JsonConvert.SerializeObject(userName);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var http = _apiService.GetHttpClient();
                var response = await http.PostAsync(url, content);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserModel>(resultJson);

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

    }
}
