using System;
using System.Collections.Generic;

namespace NextPark.Services
{
    public interface IPushNotificationService
    {
        void SendNotificationAsync(string message, List<string> tags = null);
        void SendScheduledNotificationAsync(string message, DateTime dateTime, List<string> tags = null);
    }
}

