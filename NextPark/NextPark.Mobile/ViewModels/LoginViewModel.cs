using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter;
using NextPark.Enums.Enums;
using NextPark.Mobile.Services;
using NextPark.Models;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace NextPark.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string LoginName { get; set; }       // Username text
        public string Password { get; set; }        // Password text

        public bool IsRunning { get; set; }         // Activity spinner

        public ICommand OnLoginClick { get; set; }      // Login action
        public ICommand OnRegisterClick { get; set; }   // Register action

        // SERVICES
        private readonly IDialogService _dialogService;

        // METHODS
        public LoginViewModel(IDialogService dialogService,
                                 IApiService apiService,
                                 IAuthService authService,
                                 INavigationService navService)
                                 : base(apiService, authService, navService)
        {
            _dialogService = dialogService;

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnLoginClick = new Command<object>(OnLoginClickMethod);
            OnRegisterClick = new Command<object>(OnRegisterClickMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            /*
            if (data != null)
            {
                return Task.FromResult(false);
            }
            */

            // Header
            BackText = "Indietro";
            UserName = "Accedi";
            UserMoney = "0";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            return Task.FromResult(false);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            // TODO: go back to previus view
            NavigationService.NavigateToAsync<HomeViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to profile, do nothing?)
            //NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to money page, do nothing?)
            //NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Login button action
        public void OnLoginClickMethod(object sender)
        {
            // TODO: fill data according to backend login method
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            // TODO: call login method
            LoginMethod();
        }

        // Register button action
        public void OnRegisterClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<RegisterViewModel>();
        }

        public async void LoginMethod()
        {
            
            try
            {
                // Getting device information for push notification
                var deviceId = await AppCenter.GetInstallIdAsync().ConfigureAwait(false);
                var platform = Device.RuntimePlatform;
                var loginModel = new LoginModel
                {
                    UserName = LoginName,
                    Password = Password,
                    DeviceId = deviceId.ToString(),
                    Platform = platform == Device.Android ? DevicePlatform.Android : DevicePlatform.Ios
                };

                // Send login request to backend
                var loginResponse = await AuthService.Login(loginModel);

                // Check login result
                if (loginResponse.IsSuccess == true)
                {
                    var userResponse = await AuthService.GetUserByUserName(LoginName);

                    // Stop activity spinner
                    IsRunning = false;
                    base.OnPropertyChanged("IsRunning");

                    // Check user data response
                    if (userResponse.IsSuccess == true) 
                    {
                        await NavigationService.NavigateToAsync<HomeViewModel>();
                    } 
                    else
                    {
                        // TODO: manage this case, use another message to user
                        await _dialogService.ShowAlert("Attenzione", "Errore durante il caricamento dei dati");
                    }
                }
                else
                {
                    // Stop activity spinner
                    IsRunning = false;
                    base.OnPropertyChanged("IsRunning");

                    await _dialogService.ShowAlert("Attenzione", "Accesso fallito");
                }
            } catch (Exception ex) {
                // TODO: manage exception, remove follwoing debug message!
                //await _dialogService.ShowAlert("Errore", ex.Message);
                await _dialogService.ShowAlert("Attenzione", "Accesso fallito");
            }
        }

    }
}
