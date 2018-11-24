using System.Threading.Tasks;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Views.EditProfile;
using Inside.Xamarin.Views.Event;
using Inside.Xamarin.Views.Login;
using Inside.Xamarin.Views.Master;
using Inside.Xamarin.Views.Parking;
using Inside.Xamarin.Views.ParkingRent;
using Inside.Xamarin.Views.RegisterUser;
using Xamarin.Forms;

namespace Inside.Xamarin.Services
{
    public class NavigationService
    {
        private static NavigationService Instance { get; set; }


        public void SetMainPage(string pageName)
        {
            switch (pageName)
            {
                case Pages.LoginView:
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                    break;
                case Pages.MasterView:
                    Application.Current.MainPage = new MasterView();
                    break;
            }
        }

        public async Task NavigateOnMaster(string pageName)
        {
            App.Master.IsPresented = false;

            switch (pageName)
            {
                case Pages.ParkingCreate:
                    await App.Navigator.PushAsync(new ParkingCreate());
                    break;
                case Pages.ParkingRentView:
                    await App.Navigator.PushAsync(new ParkingRentPage());
                    break;
                case Pages.ParkingEdit:
                    await App.Navigator.PushAsync(new ParkingEditPage());
                    break;
                case Pages.EventView:
                    await App.Navigator.PushAsync(new EventPage());
                    break;
                case Pages.EditProfile:
                    await App.Navigator.PushAsync(new EditProfile());
                    break;
                case Pages.ChangePassword:
                    await App.Navigator.PushAsync(new ChangePasswordPage());
                    break;
            }
        }

        public async Task NavigateOnLogin(string pageName)
        {
            switch (pageName)
            {
                case Pages.LoginView:
                    await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
                    break;
                case Pages.RegisterView:
                    await Application.Current.MainPage.Navigation.PushAsync(new RegisterUserPage());
                    break;
            }
        }

        public void FocusCoinsTab()
        {
            MessagingCenter.Send(this, Messages.FocusCoinsTab);
        }

        public async Task BackOnMaster()
        {
            await App.Navigator.PopAsync();
        }

        public async Task BackOnLogin()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public static NavigationService GetInstance()
        {
            return Instance ?? new NavigationService();
        }
    }

    public static class Pages
    {
        public const string LoginView = "LoginView";
        public const string RegisterView = "RegisterView";
        public const string MasterView = "MasterView";
        public const string ParkingCreate = "ParkingCreate";
        public const string ParkingRentView = "ParkingRentView";
        public const string ParkingEdit = "ParkingEdit";
        public const string EventView = "EventView";
        public const string EditProfile = "EditProfile";
        public const string ChangePassword = "ChangePassword";
    }
}