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
        public string ParkingsStatus { get; set; }          // Free parkings / Tot. parkings + free
        public string ParkingsAvailability { get; set; }    // Available parkings / Tot. parkings + available
        public ICommand OnParkingsAction { get; set; }      // Parkings selection action
        // User bookings
        public string NextBooking { get; set; }         // Next booking descriprion, or no bookings
        public ICommand OnBookingsAction { get; set; }  // Bookings selection action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IOrderDataService _orderDataService;

        // METHODS
        public UserProfileViewModel(IDialogService dialogService,
                                    IApiService apiService, 
                                    IAuthService authService, 
                                    INavigationService navService,
                                    IParkingDataService parkingDataService,
                                    IOrderDataService orderDataService
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
        
            var parkingList = await _parkingDataService.GetAllParkingsAsync();

            //if (parkingList.Count > 0) _parkingDataService.Parkings = parkingList;

            // Reset counters
            _totUserParkings = 0;
            _activeUserParkings = 0;

            // Search user parkings 
            // TODO: use a filter on Parkings
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
                var orderList = await _orderDataService.GetAllOrdersAsync();
                if (orderList != null)
                {
                    if (orderList.Count > 0)
                    {

                        OrderModel nextOrder = null;

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
                // TODO: manage exception
                return false;
            }
            return false;
        }
    }
}
