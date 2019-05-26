using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NextPark.Mobile.Views;
using NextPark.Mobile.Settings;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace NextPark.Mobile
{
    public partial class App : Application
    {
        public static bool IsInForeground { get; set; } = false;

        public App()
        {
            InitializeComponent();
            InitializeApp();

            MainPage = new LaunchScreen();

            //Change to the test page for api services testing. 
            //MainPage = new TestPage();

        }

        private void InitializeApp()
        {
            IoCSettings.RegisterDependencies();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            IsInForeground = true;

            // This should come before AppCenter.Start() is called
            // Avoid duplicate event registration:
            if (!AppCenter.Configured)
            {
                Push.PushNotificationReceived += PushSettings.OnPushNotificationReceived;                    
            }

            // Start AppCenter
            //AppCenter.Start("bb5bafb8-28fb-459f-b36c-647cdbd705ff", typeof(Push));
            AppCenter.Start("android=dfc399a3-85b9-4d02-9bb6-3c253980fb71;" +
                            "ios=bb5bafb8-28fb-459f-b36c-647cdbd705ff",
                            typeof(Push));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            IsInForeground = false;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            IsInForeground = true;
        }
    }
}
