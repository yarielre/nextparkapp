using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Domain.Entities;
using NextPark.Models.Models.PushNotification;

namespace NextPark.Services
{
    public interface IPushNotificationService
    {
        string Android { get; }
        string ApiKey { get; }
        string ApiKeyName { get; }
        string DeviceTarget { get; }
        string IOS { get; }
        string Organization { get; }
        string Url { get; }

        Task<PushResponse> Notify(ApplicationUser user, string name, string title, string body, IDictionary<string, string> payload = null);
        Task NotifyAll(string name, string title, string body);
        Task<PushResponse> NotifyParkingOrderExpiration(ApplicationUser user);
        Task<PushResponse> NotifyParkingOrderExpirationBeforeDeadline(ApplicationUser user);
        Task<PushResponse> NotifyParkingOwnerThatHasAnHost(ApplicationUser user);
    }
}