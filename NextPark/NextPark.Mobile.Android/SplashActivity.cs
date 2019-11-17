using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Acr.UserDialogs;
using Plugin.InAppBilling;
using Android.Content;
using Xamarin.Forms;
using NextPark.Mobile.ViewModels;
using Android.Views;
using Android.Support.V7.App;
using System.Threading.Tasks;

namespace NextPark.Mobile.Droid
{
    [Activity(Theme = "@style/MainTheme.Splash", Icon = "@mipmap/icon_sq", RoundIcon = "@mipmap/icon", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            await Task.Delay(50); // Simulate a bit of startup work.
            StartActivity(new Intent(ApplicationContext, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }
    }
}
