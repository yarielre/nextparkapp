using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Inside.Xamarin.Droid.Constants
{
   public static class Constants
    {
        public static string SenderID = "160480912857"; //Id Mittente
        public static string ListenConnectionString = "Endpoint=sb://insideparking.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=CCdEnxEEL+HDY+AARr3wfBnslLwLdBwBHTAEjaKpgRk=";
        public static string NotificationHubName = "InsideParkingHub";
    }
}