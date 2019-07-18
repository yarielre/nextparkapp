using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NextPark.Mobile.Views;
using NextPark.Mobile.Settings;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using NextPark.Mobile.ViewModels;

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
            // Android: AppCenter.Start("cb623cc2-d661-42df-8827-17095b944984", typeof(Push));
            // iOS: AppCenter.Start("bb5bafb8-28fb-459f-b36c-647cdbd705ff", typeof(Push));
            AppCenter.Start("android=cb623cc2-d661-42df-8827-17095b944984;" +
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

            if (Application.Current.MainPage is UserBookingPage)
            {
                MessagingCenter.Send<UserBookingPage>((NextPark.Mobile.Views.UserBookingPage)Application.Current.MainPage, "RefreshData");
            }
            else if (Application.Current.MainPage is UserParkingPage)
            {
                MessagingCenter.Send<UserParkingPage>((NextPark.Mobile.Views.UserParkingPage)Application.Current.MainPage, "RefreshData");
            }
            else if (Application.Current.MainPage is UserProfilePage)
            {
                MessagingCenter.Send<UserProfilePage>((NextPark.Mobile.Views.UserProfilePage)Application.Current.MainPage, "RefreshData");
            }
            else if (Application.Current.MainPage is MoneyPage)
            {
                MessagingCenter.Send<MoneyPage>((NextPark.Mobile.Views.MoneyPage)Application.Current.MainPage, "RefreshData");
            }
            else if (Application.Current.MainPage is HomePage)
            {
                MessagingCenter.Send<HomePage>((NextPark.Mobile.Views.HomePage)Application.Current.MainPage, "RefreshData");
            }
        }
    }
}
