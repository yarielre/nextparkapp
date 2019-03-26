using System.Threading.Tasks;
using NextPark.Domain.Entities;

namespace NextPark.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}