using System;
using System.Windows.Input;
using System.Threading.Tasks;

using NextPark.Mobile.Services;
using NextPark.Mobile.Core.Settings;

using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        // PROPERTIES
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action
        // User data
        public string Lastname { get; set; }            // Lastname
        public string Address { get; set; }             // Address, NPA and City
        public string Email { get; set; }               // E-mail
        public string FullPlate { get; set; }           // Plate: plate
        public ICommand OnUserDataAction { get; set; }  // User data selection action
        // User budget
        public string FullMoney { get; set; }           // Money value + currency
        public ICommand OnBudgetAction { get; set; }    // User budget selection action
        // User parkings
        public string ParkingsStatus { get; set; }          // Free parkings / Tot. parkings + free
        public string ParkingsAvailability { get; set; }    // Available parkings / Tot. parkings + available
        public ICommand OnParkingsAction { get; set; }      // Parkings selection action
        // User bookings
        public string NextBooking { get; set; }         // Next booking descriprion, or no bookings
        public ICommand OnBookingsAction { get; set; }  // Bookings selection action

        // SERVICES
        private readonly IDialogService _dialogService;

        // METHODS
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

            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

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
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // User Data
            if (AuthService.IsUserAuthenticated())
            {
                Lastname = AuthSettings.User.Lastname;
                Email = AuthSettings.User.Email;
                Address = AuthSettings.User.Address;
                FullPlate = "Targa: " + AuthSettings.User.CarPlate;
            }
            base.OnPropertyChanged("Lastname");
            base.OnPropertyChanged("Email");
            base.OnPropertyChanged("Address");
            base.OnPropertyChanged("FullPlate");

            // Budget
            FullMoney = UserMoney + " CHF";
            base.OnPropertyChanged("FullMoney");

            // Parkings
            ParkingsStatus = "1/1 liberi";
            ParkingsAvailability = "1/1 disponibili";
            base.OnPropertyChanged("ParkingsStatus");
            base.OnPropertyChanged("ParkingsAvailability");

            // Bookings
            NextBooking = "nessuna prenotazione";
            base.OnPropertyChanged("NextBooking");

            return Task.FromResult(false);
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<HomeViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // User Data Click action
        public void OnUserDataClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserDataViewModel>();
        }

        // Budget Click action
        public void OnBudgetClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Parkings Click action
        public void OnParkingsClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserParkingViewModel>();
        }

        // Budget Click action
        public void OnBookingsClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserBookingViewModel>();
        }
    }
}
