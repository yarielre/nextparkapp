using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.Services
{
    public interface IAuthService
    {
        TokenResponse GenerateJwtTokenAsync(string email, ApplicationUser user);
    }
}