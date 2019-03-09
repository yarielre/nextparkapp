using System.Threading.Tasks;
using NextPark.Domain.Entities;

namespace NextPark.Services
{
    public interface IEmailSender
    {
        void SendDebugMessage(string Controller, string Method, string Message);
        void SendDemoEmail(string recipient, string username, string password);
        Task SendEmailAsync(string email, string subject, string message);
        void SendEmailToRegisteredUserAsync(ApplicationUser user);
    }
}