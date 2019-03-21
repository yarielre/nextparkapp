using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.Services.Services.Interfaces
{
    public interface IAuthService
    {
        TokenResponse GenerateJwtTokenAsync(string email, ApplicationUser user);
    }
}