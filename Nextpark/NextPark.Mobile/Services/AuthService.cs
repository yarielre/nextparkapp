using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Enums.Enums;
using NextPark.Mobile.Settings;
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
            AuthSettings.UserCoin = 0;
            AuthSettings.User = new UserModel { Name = "Accedi" };
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

                if (response.StatusCode != HttpStatusCode.OK) {
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                }
                
                var resultJson = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TokenResponse>(resultJson);

                if (result == null) {
                    return new TokenResponse
                    {
                        IsSuccess = false
                    };
                }

                AuthSettings.Token = null;
                AuthSettings.UserId = null;
                AuthSettings.UserName = "Accedi";
                AuthSettings.UserCoin = 0;
                AuthSettings.User = new UserModel { Name = "Accedi" };

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

                Authenticated = true;
                AuthSettings.Token = result.AuthToken;
                AuthSettings.UserId = result.UserId.ToString();

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
        public async Task<ApiResponse> GetUserByUserName(string userName)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse
                {
                    Message = isConneted.Message,
                    Result = isConneted.Result,
                    ErrorType = ErrorType.InternetConnectionError
                };
            }

            try
            {
                var url = $"{AuthEndPoint}/userbyname";
                var json = JsonConvert.SerializeObject(userName);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var http = _apiService.GetHttpClient();
                var response = await http.PostAsync(url, content);
                if (response.StatusCode != HttpStatusCode.OK)
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserModel>(resultJson);

                Authenticated = true;
                AuthSettings.User = result;
                AuthSettings.UserName = result.UserName;
                AuthSettings.UserCoin = result.Balance;

                return new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
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
