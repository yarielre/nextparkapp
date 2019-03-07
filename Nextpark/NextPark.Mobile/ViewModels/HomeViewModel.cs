using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;
using NextPark.Mobile.Services.Data;
using NextPark.Models;
using System.Collections.Generic;
using NextPark.Enums;
using NextPark.Mobile.Settings;
using NextPark.Enums.Enums;
using NextPark.Mobile.CustomControls;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NextPark.Mobile.UIModels;

namespace NextPark.Mobile.ViewModels
{

    public class HomeViewModel : BaseViewModel
    {
        // PROPERTIES
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public ImageSource UserImage { get; set; }  // Header user image
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string StatusText { get; set; }
        public string PriceText { get; set; }
        public string Picture { get; set; }
        public int UID { get; set; }
        public bool InfoPanelVisible { get; set; }  // Parking Info Panel visibility

        public CustomControls.CustomMap Map { get; set; }  // Custom Map
        public ICommand OnBookingTapped { get; set; }       // Booking button click action

        public ICommand OnSearch { get; set; }
        public string SearchText { get; set; }
        public ICommand OnCurrentPosition { get; set; }

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IEventDataService _eventDataService;
        private readonly IOrderDataService _orderDataService;
        private readonly InAppPurchaseService _inAppPurchaseService;
        private readonly IProfileService _profileService;

        private readonly IAuthService _authService;

        // PRIVATE VARIABLES
        /* FUTURE IMPLEMENTATION
        private ObservableCollection<UIParkingModel> parkings;
        public ObservableCollection<UIParkingModel> Parkings
        {
            get { return parkings; }

            //set => SetValue(ref parkings, value); TODO: Use this.
            set { parkings = value; base.OnPropertyChanged("Parkings"); }
        }
        */
        private static bool connected;
        private static bool mapReady;

        // METHODS
        public HomeViewModel(IGeolocatorService geolocatorService,
                             IDialogService dialogService,
                             IApiService apiService,
                             IAuthService authService,
                             INavigationService navService,
                             IParkingDataService parkingDataService,
                             IEventDataService eventDataService,
                             IOrderDataService orderDataService,
                             InAppPurchaseService inAppPurchaseService,
                             IProfileService profileService)
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _eventDataService = eventDataService;
            _orderDataService = orderDataService;
            _authService = authService;
            _inAppPurchaseService = inAppPurchaseService;
            _profileService = profileService;

            // Header actions
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);

            // Map action
            OnSearch = new Command<object>(OnSearchMethod);
            OnCurrentPosition = new Command(OnCurrentPositionMethod);

            InfoPanelVisible = false;
            connected = false;
            mapReady = false;
        }

        public override Task InitializeAsync(object data = null)
        {
            Map.MapReady += Map_MapReady;
            Map.Tapped += Map_Tapped;
            Map.PinTapped += Map_PinTapped;
            Map.PropertyChanged += Map_PropertyChanged;

            // Set User data
            UserName = AuthSettings.User.Name;
            UserImage = "icon_no_user_256.png";
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserImage");
            base.OnPropertyChanged("UserMoney");

            InfoPanelVisible = false;
            base.OnPropertyChanged("InfoPanelVisible");

            /*
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) {
                Map_Ready_Handler();
            }
            */
            UpdateParkingList();

            return Task.FromResult(false);
        }

        void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!mapReady) return;

            CustomMap map = sender as CustomMap;
            if (e.PropertyName == "VisibleRegion") {
                _profileService.LastMapPosition = map.VisibleRegion.Center;
            }
        }


        public async void UpdateParkingList()
        {
            // Check connection
            var result = await ApiService.CheckConnection();
            if (result.IsSuccess == false) {
                _dialogService.ShowToast("Connessione ad internet assente", TimeSpan.FromSeconds(10));
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingList(); return false; });
                return;
            }

            connected = true;

            // Check user login
            if ((!AuthService.IsUserAuthenticated()) && (!string.IsNullOrEmpty(AuthSettings.UserId)) && (!string.IsNullOrEmpty(AuthSettings.User.UserName))) {
                // Update User Data
                try {
                    var userResult = await _authService.GetUserByUserName(AuthSettings.User.UserName);
                    if (userResult.IsSuccess == false) {
                        _dialogService.ShowToast("Manutenzione in corso", TimeSpan.FromSeconds(10));
                        Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingList(); return false; });
                        return;
                    }
                    if (AuthService.IsUserAuthenticated()) {
                        // Update user data
                        UserName = AuthSettings.User.Name;
                        UserImage = "icon_no_user_256.png";
                        UserMoney = AuthSettings.UserCoin.ToString("N0");
                        base.OnPropertyChanged("UserName");
                        base.OnPropertyChanged("UserImage");
                        base.OnPropertyChanged("UserMoney");
                    }
                } catch (Exception e) {
                    _dialogService.ShowToast(e.Message, TimeSpan.FromSeconds(10));
                    Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingList(); return false; });
                }
            }

            // Get Parking list
            try
            {
                var parkingsResult = await _parkingDataService.GetAllParkingsAsync();
                if ((parkingsResult == null) || (parkingsResult.Count == 0)) return;

                /* FUTURE IMPLEMENTATION
                Parkings.Clear();
                */

                // Clear parking lists
                _profileService.ParkingList.Clear();
                _profileService.UserParkingList.Clear();

                foreach (ParkingModel parking in parkingsResult)
                {
                    UIParkingModel uiParking = new UIParkingModel(parking);

                    /* FUTURE IMPLEMENTATION
                    // Update Carousel
                    Parkings.Add(uiParking);
                    */

                    // Update parking lists
                    _profileService.ParkingList.Add(uiParking);
                    if (AuthService.IsUserAuthenticated() && (uiParking.UserId == AuthSettings.User.Id)) {
                        _profileService.UserParkingList.Add(uiParking);
                    }
                }

                // Get Events
                var eventsResult = await _eventDataService.GetAllEventsAsync();
                if (eventsResult.Count == 0) return;
                foreach(EventModel availability in eventsResult)
                {
                    UIParkingModel parkingModel = _profileService.GetParkingById(availability.ParkingId);
                    if (parkingModel != null) {
                        parkingModel.Events.Add(availability);
                    }
                }

                // Get orders
                var ordersResult = await _orderDataService.GetAllOrdersAsync();
                if ((ordersResult != null) && (ordersResult.Count != 0))
                {
                    foreach (OrderModel order in ordersResult)
                    {
                        UIParkingModel parkingModel = _profileService.GetParkingById(order.ParkingId);
                        if (parkingModel != null)
                        {
                            parkingModel.Orders.Add(order);
                        }
                    }
                }

                foreach (UIParkingModel parking in _profileService.ParkingList)
                {
                    // Add Map Pin
                    CreatePin(new Position(parking.Latitude, parking.Longitude), parking);
                }
            }
            catch (Exception e) {
                _dialogService.ShowToast(e.Message, TimeSpan.FromSeconds(10));
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingList(); return false; });
            }
        }

        private void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Position, Distance.FromKilometers(1)));
        }

        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            UIParkingModel uiParking = _profileService.GetParkingById(e.Parking.Id);
            if (uiParking != null) {
                // Parking found

                // Update Info panel data
                if (string.IsNullOrEmpty(uiParking.ImageUrl))
                {
                    Picture = "icon_no_photo.png";
                }
                else
                {
                    Picture = ApiSettings.BaseUrl + uiParking.ImageUrl;
                }
                StatusText = (uiParking.isFree()) ? "Disponibile" : "Occupato";
                PriceText = uiParking.PriceMin.ToString("N2") + " CHF/h";
                UID = uiParking.Id;

                InfoPanelVisible = true;

                base.OnPropertyChanged("Picture");
                base.OnPropertyChanged("StatusText");
                base.OnPropertyChanged("PriceText");
                base.OnPropertyChanged("UID");
                base.OnPropertyChanged("InfoPanelVisible");
            }
        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {

            Map_Ready_Handler();
        }

        private async void Map_Ready_Handler()
        {
            try
            {
                mapReady = true;
                if ((_profileService.LastMapPosition == null) || (_profileService.LastMapPosition == new Position(0,0)))
                {
                    _profileService.LastMapPosition = new Position(0, 0);

                    var geoLocation = await _geoLocatorService.GetLocation();

                    if (geoLocation == null) return;

                    _profileService.LastMapPosition = geoLocation.ToXamMapPosition();
                }

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(_profileService.LastMapPosition, Distance.FromKilometers(1)));

            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }
        }

        private void CreatePin(Position position, UIParkingModel parking)
        {
            var pin = new CustomPin
            {
                Id = parking.Id,
                Parking = parking,
                Type = PinType.Place,
                Position = position,
                Label = parking.Address,
                Address = parking.Cap.ToString() + " " + parking.City,
                Icon = (parking.isFree()) ? "icon_pin_green_256" : "icon_pin_red_256"
            };

            Map.Pins.Add(pin);
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            if (_authService.IsUserAuthenticated())
            {
                NavigationService.NavigateToAsync<UserProfileViewModel>();
            }
            else if ((AuthSettings.UserId != null) && (AuthSettings.UserName != null) && (connected == false))
            {
                // No internet connection
                _dialogService.ShowAlert("Attenzione","Connessione ad internet assente");
            }
            else {
                NavigationService.NavigateToAsync<LoginViewModel>();
            }
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            if (_authService.IsUserAuthenticated())
            {
                try
                {
                    //NavigationService.NavigateToAsync<MoneyViewModel>();
                    GoToMoneyPage();
                } catch (Exception ex) {}
            }
            else if ((AuthSettings.UserId != null) && (AuthSettings.UserName != null) && (connected == false))
            {
                // No internet connection
                _dialogService.ShowAlert("Attenzione", "Connessione ad internet assente");
            }
            else
            {
                NavigationService.NavigateToAsync<LoginViewModel>();
            }

        }

        public async void GoToMoneyPage()
        {
            try
            {
                await NavigationService.NavigateToAsync<MoneyViewModel>();
            } catch (Exception ex){}
        }

        // Booking Tap action
        public void OnBookingTappedMethod(object id)
        {
            if (_authService.IsUserAuthenticated())
            {
                // Reservation
                UIBookingModel booking = new UIBookingModel {
                    StartDate = DateTime.Now.AddHours(1),
                    EndDate = DateTime.Now.AddHours(2),
                    ParkingId = (int)id
                };
                if (booking != null) 
                {
                    NavigationService.NavigateToAsync<ReservationViewModel>(booking);
                }

            // Book Now
            /*
            UIParkingModel parking = _profileService.GetParkingById((int)id);
            if (parking != null) {
                NavigationService.NavigateToAsync<BookingViewModel>(parking);
            }
            */
        }
            else
            {
                NavigationService.NavigateToAsync<LoginViewModel>();
            }
        }

        public void OnSearchMethod(object data)
        {
            SearchAddress(SearchText);
        }

        public async void SearchAddress(string address)
        {
            try
            {
                var result = await _geoLocatorService.GetPositionForAddress(SearchText);
                if (result == null)
                {
                    // no results found
                    _dialogService.ShowToast("Nessun risulato trovato");
                }

                foreach (var location in result)
                {
                    Xamarin.Forms.Maps.Position position = location.ToXamMapPosition();
                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
                    return;
                }
            } catch (Exception e) {
                _dialogService.ShowToast("Nessun risulato trovato");
            }
        }

        public void OnCurrentPositionMethod()
        {
            MoveToCurrentPosition();
        }

        public async void MoveToCurrentPosition()
        {
            try
            {
                _profileService.LastMapPosition = new Position(0, 0);

                var geoLocation = await _geoLocatorService.GetLocation();

                if (geoLocation == null) return;

                _profileService.LastMapPosition = geoLocation.ToXamMapPosition();

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(_profileService.LastMapPosition, Distance.FromKilometers(1)));

            } catch (Exception e) {}
        }

        private async Task TestPaymentAsync()
        {

            var result = await _dialogService.ShowConfirmAlert("Map Tapped", "Test the payment?");

            if (result)
            {
                var purchaseResult = this._inAppPurchaseService.MakePurchase();


            }
        }
    }
}
