using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Domain.Entities;
using NextPark.Models.Models.PushNotification;

namespace NextPark.Services.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task<PushResponse> NotifyAsync(ApplicationUser user, string name, string title, string body, IDictionary<string, string> payload = null);
        Task NotifyAllAsync(string name, string title, string body);
    }
}

