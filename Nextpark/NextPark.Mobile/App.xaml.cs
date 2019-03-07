using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NextPark.Mobile.Views;
using NextPark.Mobile.Infrastructure;
using NextPark.Mobile.Settings;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace NextPark.Mobile
{
    public partial class App : Application
    {

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
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
