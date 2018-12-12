using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NextPark.Mobile.Views;
using NextPark.Mobile.Infrastructure;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace NextPark.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            InitializeApp();

            MainPage = new HomePage();
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