using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;

namespace NextPark.Mobile.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;

        public UserProfileViewModel(IDialogService dialogService,
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
            // Profile list actions
            OnUserDataAction = new Command(OnUserDataClickMethod);
            OnBudgetAction = new Command(OnBudgetClickMethod);
            OnParkingsAction = new Command(OnParkingsClickMethod);
            OnBookingsAction = new Command(OnBookingsClickMethod);

            UserName = "Accedi";
            UserMoney = "0";

        }

        public override Task InitializeAsync(object data = null)
        {
            /*
            if (data == null)
            {
                return Task.FromResult(false);
            }
            */
            UserName = "Jonny";
            UserMoney = "8";
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            return Task.FromResult(false);
        }

        // Back Click Action
        public ICommand OnBackClick { get; set; }
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<HomeViewModel>();
        }

        // User Text and Click action
        public string UserName { get; set; }
        public ICommand OnUserClick { get; set; }
        public void OnUserClickMethod(object sender)
        {
        }

        // Money Text and Click action
        public string UserMoney { get; set; }
        public ICommand OnMoneyClick { get; set; }
        public void OnMoneyClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "Money");
        }

        // User Data Click action
        public ICommand OnUserDataAction { get; set; }
        public void OnUserDataClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "UserData");
        }

        // Budget Click action
        public ICommand OnBudgetAction { get; set; }
        public void OnBudgetClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "Budget");
        }

        // Parkings Click action
        public ICommand OnParkingsAction { get; set; }
        public void OnParkingsClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "Parkings");
        }

        // Budget Click action
        public ICommand OnBookingsAction { get; set; }
        public void OnBookingsClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "Bookings");
        }
    }
}
