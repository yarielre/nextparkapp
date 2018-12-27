using System;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class UserDataViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        //public string UserName { get; set; } // Already declared for header
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string NPA { get; set; }
        public string City { get; set; }
        public string Plate { get; set; }

        private static bool activity = false;

        public UserDataViewModel(IDialogService dialogService,
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
            OnSaveClick = new Command<object>(OnSaveClickMethod);
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
            BackText = "Profilo";
            UserName = "Jonny";
            UserMoney = "8";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // User Data
            // TODO: insert user data
            // Password
            // PasswordConfirm
            // Name
            // Surname
            // Address
            // NPA
            // City
            // Plate

            return Task.FromResult(false);
        }

        // Back Click Action
        public string BackText { get; set; }
        public ICommand OnBackClick { get; set; }
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Text and Click action
        public string UserName { get; set; }
        public ICommand OnUserClick { get; set; }
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Text and Click action
        public string UserMoney { get; set; }
        public ICommand OnMoneyClick { get; set; }
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        public bool IsRunning { get; set; }
        public ICommand OnSaveClick { get; set; }
        public void OnSaveClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "Save");
            if (activity == true) activity = false;
            else activity = true;

            IsRunning = activity;
            base.OnPropertyChanged("IsRunning");
        }
    }
}
