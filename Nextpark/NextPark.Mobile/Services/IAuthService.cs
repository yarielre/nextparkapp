using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IAuthService
    {
        Task<Response> GetUserByUserName(string endpoint, string userName);
        bool IsUserAuthenticated();
        Task<TokenResponse> Login(string endpoint, string username, string password);
        Task<TokenResponse> Logout(string endpoint);
        Task<TokenResponse> Register(string endpoint, RegisterModel model);
    }
}