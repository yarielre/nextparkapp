using Inside.Xamarin.Helpers;
using NextPark.Models;
using System.Threading.Tasks;

namespace Inside.Xamarin.Services
{

    public class AuthService
    {
        public ApiService InsideApi;

        public AuthService()
        {
            this.InsideApi = new ApiService();
        }

        public async Task<LoginResponse> Login(LoginModel model) {


            var tokenResponse = await InsideApi.Post<LoginModel, TokenResponse>(HostSetting.LoginEndpoint, model);

            if (tokenResponse != null &&  tokenResponse.IsSuccess) {

                var loggerUserResponse = await this.InsideApi.GetUserByUserName(HostSetting.AuthEndPoint, model.UserName);

                if (loggerUserResponse != null && loggerUserResponse.IsSuccess)
                {
                    return new LoginResponse
                    {
                        IsSuccess = loggerUserResponse.IsSuccess,
                        Message = "OK",
                        AuthToken = (tokenResponse.Result as TokenResponse).AuthToken,
                        User = loggerUserResponse.Result as UserModel
                    };
                }

                return new LoginResponse
                {
                    IsSuccess = loggerUserResponse.IsSuccess,
                    Message = loggerUserResponse.Message
                };

            }

            return new LoginResponse
            {
                IsSuccess = tokenResponse.IsSuccess,
                Message = tokenResponse.Message
            };
        }
    }

    public class LoginResponse  {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string AuthToken { get; set; }
        public UserModel User { get; set; }
    }
}