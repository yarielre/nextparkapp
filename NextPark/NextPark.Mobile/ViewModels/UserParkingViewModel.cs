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
using NextPark.Enums.Enums;
using System.ComponentModel;
using NextPark.Mobile.UIModels;

namespace NextPark.Mobile.ViewModels
{
    public class ParkingItem : INotifyPropertyChanged
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

        private int cap;
        public int Cap
        {
            get { return cap; }
            set { cap = value; }
        }

        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        private Color statusColor;
        public Color StatusColor
        {
            get { return statusColor; }
            set { statusColor = value; } 
        }

        private string picture;
        public string Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        private ICommand onParkingTap;
        public ICommand OnParkingTap
        {
            get { return onParkingTap; }
            set { onParkingTap = value; }
        }

        public ParkingModel ParkingModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UserParkingViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public bool NoElementFound { get; set; }
        public double ParkingListHeight { get; set; }   // Parking List View Height Request

        public ICommand OnAddParking { get; set; }      // Add parking button action
        public ICommand OnParkingTapped { get; set; }   // Parking list tap action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IOrderDataService _orderDataService;
        private readonly IEventDataService _eventDataService;
        private readonly IProfileService _profileService;

        // PRIVATE VARIABLES
        private ObservableCollection<ParkingItem> _parkingList;

        public ObservableCollection<ParkingItem> ParkingList
        {
            get { return _parkingList; }
            set { _parkingList = value; base.OnPropertyChanged("ParkingList"); }
        }

        // METHODS
        public UserParkingViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService,
                                    IParkingDataService parkingDataService,
                                    IOrderDataService orderDataService,
                                    IEventDataService eventDataService,
                                    IProfileService profileService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;
            _eventDataService = eventDataService;
            _profileService = profileService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnAddParking = new Command<object>(OnAddParkingMethod);
            OnParkingTapped = new Command<object>(OnParkingTappedMethod);

            ParkingList = new ObservableCollection<ParkingItem>();
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            // Header
            BackText = "Profilo";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // TODO: add parking list
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

        // Add Parking Click Action
        public void OnAddParkingMethod(object sender)
        {
            NavigationService.NavigateToAsync<AddParkingViewModel>();
            //_dialogService.ShowAlert("Alert", "Add Parking");
        }

        // Parking list item click Action
        public void OnParkingTappedMethod(object sender)
        {
            // TODO: Evaluate using UID or index of parking list (currently using UID as index)
            if (sender is int)
            {
                ParkingItem item = ParkingList[(int)sender];
                // TODO: pass parking item to parking data page
                NavigationService.NavigateToAsync<ParkingDataViewModel>(item);
                //_dialogService.ShowAlert("Alert", "Parking data: " + item.Address);
            }
        }

        // Updated UserParkings
        public async void UpdateUserParkingData()
        {
            await GetUserParkings();
        }

        private async Task GetUserParkings()
        {
            var parkingList = await _parkingDataService.GetAllParkingsAsync();

            ParkingList.Clear();

            // Search user parkings 
            // TODO: use a filter on Parkings
            if (parkingList != null)
            {
                int count = 0;
                foreach (ParkingModel parking in parkingList)
                {
                    if (parking.UserId == int.Parse(AuthSettings.UserId))
                    {
                        bool free = true;

                        // TODO: improve get orders by parking id
                        //var ordersResult = await _orderDataService.GetAllOrdersAsync();

                        UIParkingModel uiParking = _profileService.GetParkingById(parking.Id);

                        if (uiParking == null) {
                            // Add parking to user parking list
                            uiParking = new UIParkingModel(parking);

                            // Get parking availabilities
                            var eventList = await _eventDataService.GetAllEventsAsync();

                            if ((eventList != null) && (eventList.Count > 0))
                            {
                                foreach (EventModel availability in eventList)
                                {
                                    if (availability.ParkingId == uiParking.Id)
                                    {
                                        uiParking.Events.Add(availability);
                                    }
                                }

                                // Get all parking orders
                                var orderList = await _orderDataService.GetAllOrdersAsync();

                                if ((orderList != null) && (orderList.Count > 0))
                                {
                                    foreach (OrderModel order in orderList)
                                    {
                                        if (order.ParkingId == uiParking.Id)
                                        {
                                            uiParking.Orders.Add(order);
                                        }
                                    }
                                }
                            }
                            _profileService.ParkingList.Add(uiParking);
                        }

                        // Set Image
                        string imageUrl = "";
                        if (string.IsNullOrEmpty(parking.ImageUrl))
                        {
                            imageUrl = "icon_no_photo.png";
                        } else {
                            imageUrl = ApiSettings.BaseUrl + parking.ImageUrl;
                        }

                        string status = (parking.Status == ParkingStatus.Disabled) ? "Disattivo" : ((uiParking.isFree())? "Disponibile" : "Non disponibile");

                        ParkingList.Add(new ParkingItem
                        {
                            UID = parking.Id,
                            Index = count++,
                            Address = parking.Address,
                            Cap = parking.Cap,
                            City = parking.City,
                            Status = status,
                            StatusColor = (uiParking.isFree()) ? Color.Green : Color.Red,
                            Picture = imageUrl,
                            OnParkingTap = OnParkingTapped,
                            ParkingModel = parking
                        });
                    }
                }
                /*
                ParkingList.Add(new ParkingItem
                {
                    UID = 1,
                    Index = count++,
                    Address = "Via Industria 20",
                    Cap = 6928,
                    City = "Manno",
                    Status = "libero",
                    StatusColor = Color.Green,
                    OnParkingTap = OnParkingTapped
                });
                */
                base.OnPropertyChanged("ParkingList");
                if (ParkingList.Count == 0)
                {
                    NoElementFound = true;

                }
                else
                {
                    NoElementFound = false;
                }
                base.OnPropertyChanged("NoElementFound");

                ParkingListHeight = ParkingList.Count * 70.0;
                base.OnPropertyChanged("ParkingListHeight");
            }
        }
    }
}
