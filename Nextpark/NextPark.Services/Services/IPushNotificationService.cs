using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Domain.Entities;
using NextPark.Models.Models.PushNotification;

namespace NextPark.Services
{
    public interface IPushNotificationService
    {
        Task NotifyAllAsync(string name, string title, string body);
        Task<PushResponse> NotifyAsync(ApplicationUser user, string name, string title, string body, IDictionary<string, string> payload = null);
    }
}