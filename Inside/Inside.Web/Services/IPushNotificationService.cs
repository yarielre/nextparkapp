using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inside.Web.Services
{
   public interface IPushNotificationService
    {
        void SendNotificationAsync(string message, List<string> tags = null);
        void SendScheduledNotificationAsync(string message, DateTime dateTime, List<string> tags = null);
    }
}

