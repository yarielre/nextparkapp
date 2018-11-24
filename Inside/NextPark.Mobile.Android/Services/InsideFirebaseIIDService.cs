using Android.Util;
using WindowsAzure.Messaging;
using Firebase.Iid;
using Android.App;
using System.Collections.Generic;

namespace Inside.Xamarin.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class InsideFirebaseIIDService : FirebaseInstanceIdService
     {

        const string TAG = "InsideFirebaseIIDService";
        NotificationHub hub;

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "FCM token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
            // Register with Notification Hubs
            hub = new NotificationHub(Xamarin.Droid.Constants.Constants.NotificationHubName,
                                      Xamarin.Droid.Constants.Constants.ListenConnectionString, this);

            var tags = new List<string>() { };
            var regID = hub.Register(token, tags.ToArray()).RegistrationId;

            Log.Debug(TAG, $"Successful registration of ID {regID}");
        }

    }
}