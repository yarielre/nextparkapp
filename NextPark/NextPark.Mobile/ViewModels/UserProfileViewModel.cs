using System;
using System.Windows.Input;
using System.Threading.Tasks;

using NextPark.Mobile.Services;
using NextPark.Mobile.Settings;
using NextPark.Models;

using Xamarin.Forms;
using NextPark.Mobile.Services.Data;
using NextPark.Enums.Enums;

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
        public string Address { get; set; }             // Address
        public string NPA { get; set; }                 // Postal code
        public string City { get; set; }                // City
        public string Email { get; set; }               // E-mail
        public string CarPlate { get; set; }            // Car plate
        public ICommand OnUserDataAction { get; set; }  // User data selection action
        // User budget
        public string Balance { get; set; }             // Balance value
        public string Profit { get; set; }              // Profit value
        public ICommand OnBudgetAction { get; set; }    // User budget selection action
        // User parkings
        private int _totUserParkings { get; set; }
        private int _activeUserParkings { get; set; }
        private int _availableUserParkings { get; set; }

        public string ParkingsStatus { get; set; }          // Free parkings / Tot. parkings + free
        public string ParkingsAvailability { get; set; }    // Available parkings / Tot. parkings + available
        public ICommand OnParkingsAction { get; set; }      // Parkings selection action
        // User bookings
        public string NextBooking { get; set; }         // Next booking descriprion, or no bookings
        public ICommand OnBookingsAction { get; set; }  // Bookings selection action
        // Customer service
        public ICommand OnCommandClick { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IOrderDataService _orderDataService;
        private readonly IProfileService _profileService;

        // METHODS
        public UserProfileViewModel(IDialogService dialogService,
                                    IApiService apiService, 
                                    IAuthService authService, 
                                    INavigationService navService,
                                    IParkingDataService parkingDataService,
                                    IOrderDataService orderDataService,
                                    IProfileService profileService
                                   )
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;
            _profileService = profileService;

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            // Profile list actions
            OnUserDataAction = new Command(OnUserDataClickMethod);
            OnBudgetAction = new Command(OnBudgetClickMethod);
            OnParkingsAction = new Command(OnParkingsClickMethod);
            OnBookingsAction = new Command(OnBookingsClickMethod);
            OnCommandClick = new Command<string>(OnCommandClickMethod);

            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // User Data
            if (AuthService.IsUserAuthenticated())
            {
                Lastname = AuthSettings.User.Lastname;
                Email = AuthSettings.User.Email;
                Address = AuthSettings.User.Address;
                NPA = AuthSettings.User.Cap.ToString();
                City = AuthSettings.User.City;
                CarPlate = AuthSettings.User.CarPlate;
            }
            base.OnPropertyChanged("Lastname");
            base.OnPropertyChanged("Email");
            base.OnPropertyChanged("Address");
            base.OnPropertyChanged("NPA");
            base.OnPropertyChanged("City");
            base.OnPropertyChanged("CarPlate");

            // Budget
            Balance = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("Balance");
            Profit = AuthSettings.User.Profit.ToString("N0");
            base.OnPropertyChanged("Profit");

            // Parkings
            ParkingsStatus = "Caricamento...";
            ParkingsAvailability = "";
            base.OnPropertyChanged("ParkingsStatus");
            base.OnPropertyChanged("ParkingsAvailability");

            // Bookings
            NextBooking = "Caricamento...";
            base.OnPropertyChanged("NextBooking");

            UpdateUserParkingData();

            return Task.FromResult(false);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
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

        // Updated UserParkings
        public async void UpdateUserParkingData()
        {
            await GetUserParkings();
        }

        private async Task GetUserParkings()
        {
            try
            {
                // TODO: evaluate the use of profileService.UserParkingList or add a filter to parkingDataService.GetUserParkings()

                // Get parkings
                var parkingList = await _parkingDataService.GetAllParkingsAsync();

                // Reset counters
                _totUserParkings = 0;
                _activeUserParkings = 0;

                // Search user parkings 
                if (parkingList != null)
                {
                    foreach (ParkingModel parking in parkingList)
                    {
                        if (parking.UserId == int.Parse(AuthSettings.UserId))
                        {
                            _totUserParkings++;
                            if (parking.Status == ParkingStatus.Enabled)
                            {
                                _activeUserParkings++;
                            }
                            var uiParking = _profileService.GetParkingById(parking.Id);
                            if (uiParking.isFree()) {
                                _availableUserParkings++;
                            }
                        }
                    }
                }

                // Update status on page
                ParkingsStatus = _availableUserParkings.ToString() + "/" + _totUserParkings.ToString() + " disponibili";
                ParkingsAvailability = _activeUserParkings.ToString() + "/" + _totUserParkings.ToString() + " attivi";
                base.OnPropertyChanged("ParkingsStatus");
                base.OnPropertyChanged("ParkingsAvailability");

                UpdateUserBookingsData();
            } catch (Exception e) {
                return;
            }
        }

        public async void UpdateUserBookingsData()
        {
            await GetBookings();
        }

        // Get User Bookings
        public async Task<bool> GetBookings()
        {
            try {
                var orderList = await _orderDataService.GetAllOrdersAsync();
                if (orderList != null)
                {
                    OrderModel nextOrder = null;

                    if (orderList.Count > 0)
                    {                    
                        foreach (OrderModel order in orderList) 
                        {
                            if (order.UserId == int.Parse(AuthSettings.UserId))
                            {
                                if (nextOrder == null)
                                {
                                    nextOrder = order;
                                } else {
                                    if (nextOrder.StartDate > order.StartDate)
                                    {
                                        nextOrder = order;
                                    }
                                }
                            }
                        }
                    }
                    if (nextOrder != null)
                    {
                        NextBooking = nextOrder.StartDate.ToShortTimeString();
                    }
                    else
                    {
                        NextBooking = "nessuna prenotazione";
                    }
                    base.OnPropertyChanged("NextBooking");
                    return true;
                } else {
                    NextBooking = "nessuna prenotazione";
                    base.OnPropertyChanged("NextBooking");
                    return false;
                }
            } catch (Exception e)
            {
                // TODO: manage exception
                return false;
            }
        }

        public void OnCommandClickMethod(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Xamarin.Forms.Device.OpenUri(new System.Uri(url));
            }
        }
    }
}
