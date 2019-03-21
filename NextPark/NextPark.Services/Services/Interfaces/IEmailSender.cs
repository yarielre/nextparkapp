using System.Threading.Tasks;
using NextPark.Domain.Entities;

namespace NextPark.Services.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        void SendEmailToRegisteredUserAsync(ApplicationUser user);
    }
}
