using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IAuthService
    {
        bool Authenticated { get; set; }

        Task<ApiResponse> GetUserByUserName(string userName);
        bool IsUserAuthenticated();
        Task<TokenResponse> Login(LoginModel model);
        Task<TokenResponse> Logout();
        Task<TokenResponse> Register(RegisterModel model);
    }
}