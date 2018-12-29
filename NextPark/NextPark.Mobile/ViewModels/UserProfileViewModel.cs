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
        // PROPERTIES
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action
        // User data
        public string FullName { get; set; }            // Name and Surname
        public string Address { get; set; }             // Address, NPA and City
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

            UserName = "Accedi";
            UserMoney = "0";

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
            UserName = "Jonny";
            UserMoney = "8";
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // User Data
            // UserName
            FullName = "Mario Rossi";
            Address = "Via Strada 1, 6900 Lugano";
            FullPlate = "Targa TI 123456";
            base.OnPropertyChanged("FullName");
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
            _dialogService.ShowAlert("Alert", "Bookings");
        }
    }
}
