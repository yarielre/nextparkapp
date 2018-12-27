using System;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        public string RegisterName { get; set; } // Already used in header
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string NPA { get; set; }
        public string City { get; set; }
        public string Plate { get; set; }

        public RegisterViewModel(IDialogService dialogService,
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
            OnRegisterClick = new Command<object>(OnRegisterClickMethod);
        }

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

        // Back Click Action
        public string BackText { get; set; }
        public ICommand OnBackClick { get; set; }
        public void OnBackClickMethod(object sender)
        {
            // TODO: go back to previus view
        }

        // User Text and Click action
        public string UserName { get; set; }
        public ICommand OnUserClick { get; set; }
        public void OnUserClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to profile, do nothing?)
            //NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Text and Click action
        public string UserMoney { get; set; }
        public ICommand OnMoneyClick { get; set; }
        public void OnMoneyClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to money page, do nothing?)
            //NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // On Register
        public ICommand OnRegisterClick { get; set; }
        public void OnRegisterClickMethod(object sender)
        {
            // TODO: fill user data according to register data model
            // TODO: send registration request to backend
            _dialogService.ShowAlert("Alert", "TODO: Register user");
        }        
    }
}
