using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Mobile.Settings;
using NextPark.Mobile.Services.Data;
using NextPark.Models;

namespace NextPark.Mobile.ViewModels
{
    public class BookingItem
    {
        private int uid;
        public int UID
        {
            get { return uid; }
            set { uid = value; }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        private ICommand onBookingTap;
        public ICommand OnBookingTap
        {
            get { return onBookingTap; }
            set { onBookingTap = value; }
        }

        private ICommand onBookingDel;
        public ICommand OnBookingDel
        {
            get { return onBookingDel; }
            set { onBookingDel = value; }
        }
        /*
        private ICommand onBookingSwipe;
        public ICommand OnBookingSwipe
        {
            get { return onBookingSwipe; }
            set { onBookingSwipe = value; }
        }
        */
    }

    public class UserBookingViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public bool NoElementFound { get; set; }
        public double BookingListHeight { get; set; }   // Booking List View Height Request

        public ICommand OnBookingTapped { get; set; }   // Booking list tap action
        public ICommand OnBookingDelete { get; set; }   // Booking delete action
        //public ICommand OnBookingSwiped { get; set; }   // Booking swipe action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly ParkingDataService _parkingDataService;
        private readonly OrderDataService _orderDataService;

        // PRIVATE VARIABLES
        private ObservableCollection<BookingItem> bookingList;

        public ObservableCollection<BookingItem> BookingList
        {
            get { return bookingList; }
            set { bookingList = value; base.OnPropertyChanged("BookingList"); }
        }

        // METHODS
        public UserBookingViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService,
                                    ParkingDataService parkingDataService,
                                    OrderDataService orderDataService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            // Item actions
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);
            OnBookingDelete = new Command<object>(OnBookingDeleteMethod);
            //OnBookingSwiped = new Command<object>(OnBookingSwipedMethod);

            // Example starting values
            BookingList = new ObservableCollection<BookingItem>();

            NoElementFound = false;
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {

            // Header
            BackText = "Profilo";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // TODO: add booking list
            UpdateUserBookings();

            return Task.FromResult(false);
        }

        public async void UpdateUserBookings()
        {
            await GetUserBookings();
        }

        private async Task GetUserBookings()
        {
            var ordersResponse = await _orderDataService.Get();

            /*
            {
                new BookingItem { UID = 0, Address = "Via Strada 1", City = "Lugano, Ticino", Time = "01h45", OnBookingTap = OnBookingTapped, OnBookingDel = OnBookingDelete },
                new BookingItem { UID = 1, Address = "Via Strada 1.5", City = "Lugano, Ticino", Time = "02h00", OnBookingTap = OnBookingTapped, OnBookingDel = OnBookingDelete },
                new BookingItem { UID = 2, Address = "Via Strada 2", City = "Lugano, Ticino", Time = "02h30", OnBookingTap = OnBookingTapped, OnBookingDel = OnBookingDelete }
            };
            */

            int count = 0;
            BookingList.Clear();

            foreach (OrderModel order in ordersResponse)
            {
                if (order.UserId == int.Parse(AuthSettings.UserId))
                {
                    var parkingResponse = await _parkingDataService.Get(order.ParkingId);

                    if (parkingResponse != null) {

                        TimeSpan remainingTime = order.EndDate - DateTime.Now;

                        BookingList.Add(new BookingItem
                        {
                            UID = order.Id,
                            Index = count++,
                            Address = parkingResponse.Address, // get parking descrption from order.ParkingId
                            City = parkingResponse.Cap.ToString() + " " + parkingResponse.City, // get parking city order.ParkingId
                            Time = string.Format("{0}:{1}", remainingTime.Hours.ToString(), remainingTime.Minutes.ToString()),
                            OnBookingDel = OnBookingDelete,
                            OnBookingTap = OnBookingTapped
                        });
                    }

                }
            }
            base.OnPropertyChanged("BookingList");

            if (BookingList.Count == 0)
            {
                NoElementFound = true;

            }
            else
            {
                NoElementFound = false;
            }
            base.OnPropertyChanged("NoElementFound");

            BookingListHeight = BookingList.Count * 50.0;
            base.OnPropertyChanged("BookingListHeight");
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Text and Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Booking list item click action
        public void OnBookingTappedMethod(object sender)
        {
            // TODO: Evaluate using UID or index of booking list (currently using UID as index)
            if (sender is int)
            {
                BookingItem item = BookingList[(int)sender];
                // TODO: pass booking item to booking map page
                NavigationService.NavigateToAsync<BookingMapViewModel>(item);
                _dialogService.ShowAlert("Alert", "Booking data: " + item.Address);
            }
        }

        // Booking list item delete click action
        public void OnBookingDeleteMethod(object sender)
        {
            // TODO: Evaluate using UID or index of booking list (currently using UID as index)
            if (sender is int)
            {
                BookingItem item = BookingList[(int)sender];
                // TODO: pass booking item to backend delete booking method
                _dialogService.ShowAlert("Alert", "Delete booking: " + item.Address);
            }
        }

        public void OnBookingSwipedMethod(object args)
        {
            _dialogService.ShowAlert("Alert", "Swipe booking: " + args);
        }
    }
}
