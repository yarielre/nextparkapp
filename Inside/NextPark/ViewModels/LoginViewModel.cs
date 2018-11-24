using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Attributes
        private bool _isEnable;
        private bool _isRunning;
        private string _password;
        private string _username;
        private NavigationService navigationService;
        private AuthService authService;
        #endregion

        #region Properties
        public string UserName
        {
            get => _username;
            set => SetValue(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        public bool IsRemembered { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value);
        }

        public bool IsEnable
        {
            get => _isEnable;
            set => SetValue(ref _isEnable, value);
        }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            IsRemembered = true;
            IsEnable = true;
            navigationService = NavigationService.GetInstance();
            authService = new AuthService();
        }
        #endregion



        #region Commands
        public ICommand LoginCommand => new RelayCommand(Login);
        public ICommand GoToRegisterPageCommand => new RelayCommand(GoToRegisterPage);
        #endregion


        #region Methods

 
        private async void Login()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    Languages.LoginUserNameAlert,
                    Languages.GeneralAccept);
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                   Languages.GeneralError,
                    Languages.LoginPasswordAlert,
                    Languages.GeneralAccept);
                return;
            }

            LoginModel model = new LoginModel
            {
                UserName = this.UserName,
                Password = this.Password
            };
            IsRunning = true;
            var loginResponse = await authService.Login(model);
            IsRunning = false;
            if (!loginResponse.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    loginResponse.Message,
                    Languages.GeneralAccept);
            }
            else
            {
                MainViewModel.GetInstance().SetUpLoginData(loginResponse.User, loginResponse.AuthToken);
                NavigationService.GetInstance().SetMainPage(Pages.MasterView);             
            }
            this.Refresh();
        }

        private void Refresh()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
        }

        private async void GoToRegisterPage()
        {
            await navigationService.NavigateOnLogin(Pages.RegisterView);
            Refresh();
        }
        #endregion
    }
}