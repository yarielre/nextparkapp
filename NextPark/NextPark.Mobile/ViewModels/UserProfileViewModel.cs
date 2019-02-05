using System;
using System.Windows.Input;
using System.Threading.Tasks;

using NextPark.Mobile.Services;
using NextPark.Mobile.Core.Settings;
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
        public string Address { get; set; }             // Address, NPA and City
        public string Email { get; set; }               // E-mail
        public string FullPlate { get; set; }           // Plate: plate
        public ICommand OnUserDataAction { get; set; }  // User data selection action
        // User budget
        public string FullMoney { get; set; }           // Money value + currency
        public ICommand OnBudgetAction { get; set; }    // User budget selection action
        // User parkings
        private int _totUserParkings { get; set; }
        private int _activeUserParkings { get; set; }
        public string ParkingsStatus { get; set; }          // Free parkings / Tot. parkings + free
        public string ParkingsAvailability { get; set; }    // Available parkings / Tot. parkings + available
        public ICommand OnParkingsAction { get; set; }      // Parkings selection action
        // User bookings
        public string NextBooking { get; set; }         // Next booking descriprion, or no bookings
        public ICommand OnBookingsAction { get; set; }  // Bookings selection action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly ParkingDataService _parkingDataService;
        private readonly OrderDataService _orderDataService;

        // METHODS
        public UserProfileViewModel(IDialogService dialogService,
                                    IApiService apiService, 
                                    IAuthService authService, 
                                    INavigationService navService,
                                    ParkingDataService parkingDataService,
                                    OrderDataService orderDataService
                                   )
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            // Profile list actions
            OnUserDataAction = new Command(OnUserDataClickMethod);
            OnBudgetAction = new Command(OnBudgetClickMethod);
            OnParkingsAction = new Command(OnParkingsClickMethod);
            OnBookingsAction = new Command(OnBookingsClickMethod);

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

            UpdateUserParkingData();

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

        // Updated UserParkings
        public async void UpdateUserParkingData()
        {
            await GetUserParkings();
        }

        private async Task GetUserParkings()
        {
        
            var parkingList = await _parkingDataService.Get();

            if (parkingList.Count > 0) _parkingDataService.Parkings = parkingList;

            // Reset counters
            _totUserParkings = 0;
            _activeUserParkings = 0;

            // Search user parkings 
            // TODO: use a filter on Parkings
            if (_parkingDataService.Parkings != null)
            {
                foreach (ParkingModel parking in _parkingDataService.Parkings)
                {
                    if (parking.UserId == int.Parse(AuthSettings.UserId))
                    {
                        _totUserParkings++;
                        if (parking.Status == ParkingStatus.Enabled)
                        {
                            _activeUserParkings++;
                        }
                    }
                }
            }

            // Update status on page
            ParkingsStatus = _totUserParkings.ToString() + "/" + _totUserParkings.ToString() + " liberi";
            ParkingsAvailability = _activeUserParkings.ToString() + "/" + _totUserParkings.ToString() + " disponibili";
            base.OnPropertyChanged("ParkingsStatus");
            base.OnPropertyChanged("ParkingsAvailability");

            UpdateUserBookingsData();
        }

        public async void UpdateUserBookingsData()
        {
            await GetBookings();
        }

        // Get User Bookings
        public async Task<bool> GetBookings()
        {
            try {
                var orderList = await _orderDataService.Get();
                if (orderList != null)
                {
                    if (orderList.Count > 0)
                    {
                        _orderDataService.Orders = orderList;

                        OrderModel nextOrder = null;

                        foreach (OrderModel order in _orderDataService.Orders) 
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

                        if (nextOrder != null) {
                            NextBooking = nextOrder.StartDate.ToShortTimeString();
                        } else {
                            NextBooking = "nessuna prenotazione";
                        }
                        base.OnPropertyChanged("NextBooking");
                        return true;
                    }
                }
            } catch (Exception e)
            {

            }
            return false;
        }
    }
}
