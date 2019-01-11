using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NextPark.Mobile.Services;
using Xamarin.Forms;

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
            UserName = "Login";
            UserMoney = "0";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            return Task.FromResult(false);
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
            NavigationService.NavigateToAsync<MoneyViewModel>();
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
                //Demo Login OK
                var loginResponse = await AuthService.Login(LoginName, Password);

                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                if (loginResponse.IsSuccess == true)
                {
                    // get user info
                    await _dialogService.ShowAlert("Attenzione", "Accesso completato");
                }
                else
                {
                    await _dialogService.ShowAlert("Attenzione", "Accesso fallito");
                }
            } catch (Exception ex) {}
        }

    }
}
