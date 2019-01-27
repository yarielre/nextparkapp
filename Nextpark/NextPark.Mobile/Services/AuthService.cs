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
    public class AuthService : IAuthService
    {

        private readonly IApiService _apiService;
        private readonly string AuthEndPoint = ApiSettings.AuthEndPoint;
        public bool Authenticated { get; set; }

        public AuthService(IApiService apiService)
        {
            _apiService = apiService;
            Authenticated = false;
            AuthSettings.UserName = "Accedi";
            AuthSettings.UserCoin = 0;
        }

        public async Task<TokenResponse> Login(string username, string password)
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

                var url = $"{AuthEndPoint}/login";

                var http = _apiService.GetHttpClient();

                var response = await http.PostAsync(url, content);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJson);
                result.IsSuccess = true;

                Authenticated = true;
                AuthSettings.Token = result.AuthToken;
                AuthSettings.UserId = result.UserId.ToString();
                //AuthSettings.UserName = username;

                return result;
            }
            catch (Exception e)
            {
                return new TokenResponse
                {
                    IsSuccess = false
                };
            }
        }

        public async Task<TokenResponse> Logout()
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return new TokenResponse { IsSuccess = isConneted.IsSuccess };

            try
            {

                var url = $"{AuthEndPoint}/logout";

                var http = _apiService.GetHttpClient();
                var response = await http.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJson);
                result.IsSuccess = true;

                AuthSettings.Token = null;
                AuthSettings.UserId = null;
                AuthSettings.UserName = "Accedi";
                AuthSettings.UserCoin = 0;
                Authenticated = false;

                return result;

            }
            catch (Exception)
            {
                return new TokenResponse
                {
                    IsSuccess = false
                };
            }
        }

        public async Task<TokenResponse> Register(RegisterModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return new TokenResponse { IsSuccess = isConneted.IsSuccess };


            try
            {

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{AuthEndPoint}/register";

                var http = _apiService.GetHttpClient();
                var response = await http.PostAsync(url, content);
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
            catch(Exception)
            {
                return new TokenResponse
                {
                    IsSuccess = false
                };
            }
        }
        public async Task<Response> GetUserByUserName(string userName)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{AuthEndPoint}/userbyname";
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

                AuthSettings.User = result;
                AuthSettings.UserName = result.Name;
                AuthSettings.UserCoin = result.Coins;

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

        bool IAuthService.IsUserAuthenticated()
        {
            return Authenticated;
        }
    }
}
