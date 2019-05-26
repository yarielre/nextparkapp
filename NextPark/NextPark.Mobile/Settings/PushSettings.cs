using System;
using Acr.UserDialogs;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using NextPark.Mobile.Services;

namespace NextPark.Mobile.Settings
{
    public class PushSettings
    {
        public static bool NewNotification = false;
        public static bool ExpiredOrder = false;
        public static int  ExpiredOrderId = -1;
        public static PushNotificationReceivedEventArgs NotificationArgs;
        public static Guid? DeviceId;


        public PushSettings()
        {
        }

        public static async void UpdateDeviceId()
        {
            DeviceId = await AppCenter.GetInstallIdAsync();
        }

        public static void OnPushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            NewNotification = true;
            NotificationArgs = e;

            // If there is custom data associated with the notification,
            // print the entries
            if (e.CustomData != null)
            {
                foreach (var key in e.CustomData.Keys)
                {
                    if (key == "order_id") {
                        ExpiredOrder = true;
                        ExpiredOrderId = int.Parse(e.CustomData[key]);
                    }
                }
            }
        }
    }
}
