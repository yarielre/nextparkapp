using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class ParkingDataViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Address { get; set; }             // Parking address text
        public string City { get; set; }                // Parking city text
        public string ActiveStatusText { get; set; }    // Parking active status text
        public bool ActiveSwitchToggled                 // Parking active status value
        {
            get { return this.activeSwitchToggled; }
            set { activeSwitchToggled = value; OnSwitchToggleMethod(value); }
        }
        public ICommand OnEditParking { get; set; }     // Edit parking button action
        //public ICommand OnSwitchToggle { get; set; }    // Activate/Deactivate parking switch action
        public ICommand OnCalendar { get; set; }        // Calendar click action
        public ICommand OnAddAvailability { get; set; } // Add Availability click action

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private ParkingItem parking;
        private bool activeSwitchToggled;

        // METHODS
        public ParkingDataViewModel(IDialogService dialogService,
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

            // Buttons action
            OnEditParking = new Command<object>(OnEditParkingMethod);
            //OnSwitchToggle = new Command<object>(OnSwitchToggleMethod);
            OnCalendar = new Command<object>(OnCalendarMethod);
            OnAddAvailability = new Command<object>(OnAddAvailabilityMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }

            if (data is ParkingItem)
            {
                parking = (ParkingItem)data;

                // Header
                BackText = "Parcheggi";
                UserName = "Jonny";
                UserMoney = "8";
                base.OnPropertyChanged("BackText");
                base.OnPropertyChanged("UserName");
                base.OnPropertyChanged("UserMoney");

                Address = parking.Address;
                base.OnPropertyChanged("Address");
                City = parking.City;
                base.OnPropertyChanged("City");

                // Default Value
                ActiveStatusText = "Attivo";
                ActiveSwitchToggled = true;
                base.OnPropertyChanged("ActiveStatusText");
                base.OnPropertyChanged("ActiveSwitchToggled");
            }

            return Task.FromResult(false);
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserParkingViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Edit Parking Click action
        public void OnEditParkingMethod(object sender)
        {
            // TODO: Evaluate Add Parking page with element or EditParking Page to be added
            NavigationService.NavigateToAsync<AddParkingViewModel>();
        }

        // Activate/Deactivate Parking toggle switch action
        public void OnSwitchToggleMethod(bool value)
        {
            if (value)
            {
                ActiveStatusText = "Attivo";
            }
            else
            {
                ActiveStatusText = "Non Attivo";
            }
            base.OnPropertyChanged("ActiveStatusText");
        }

        // Calendar Click action
        public void OnCalendarMethod(object sender)
        {
            // TODO: Open calendar and manage selected date
            _dialogService.ShowAlert("Alert", "TODO: open calendar");
        }

        // Calendar Click action
        public void OnAddAvailabilityMethod(object sender)
        {
            // TODO: Add availability
            _dialogService.ShowAlert("Alert", "TODO: add availability");
        }
    }
}
