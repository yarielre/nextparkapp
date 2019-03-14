using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> GetUserByUserName(string userName);
        bool IsUserAuthenticated();
        Task<TokenResponse> Login(string username, string password);
        Task<TokenResponse> Logout();
        Task<TokenResponse> Register(RegisterModel model);
    }
}