using System.Linq;
using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Messaging;
using Inside.Xamarin.ViewModels;
using Xamarin.Forms;

namespace Inside.Xamarin.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class InsideFirebaseMessagingService : FirebaseMessagingService
    {

        const string TAG = "InsideFirebaseMessagingService";

        public override void OnMessageReceived(RemoteMessage message)
        {

            base.OnMessageReceived(message);

            Log.Debug(TAG, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotification(message.GetNotification().Body);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                SendNotification(message.Data.Values.First());
            }

        }

        void SendNotification(string messageBody)
        {

            MessagingCenter.Send<object, string>(this, Xamarin.Helpers.Messages.PushNotificationReceived, messageBody);

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                        .SetContentTitle("Inside Parking Message")
                        .SetSmallIcon(Resource.Drawable.ic_launcher)
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);

            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}