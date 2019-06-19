using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Domain.Entities;
using NextPark.Models.Models.PushNotification;

namespace NextPark.Services
{
    public interface IPushNotificationService
    {
        Task<PushResponse> Notify(ApplicationUser user, string name, string title, string body, IDictionary<string, string> payload = null);
        Task NotifyAll(string name, string title, string body);
    }
}