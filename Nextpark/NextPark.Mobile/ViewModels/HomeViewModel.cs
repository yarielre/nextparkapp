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
using Plugin.Permissions.Abstractions;

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
        public string BookingBtnText { get; set; }
        public bool BookingBtnEnable { get; set; }
        public Color BookingBtnBackground { get; set; }
        public Color BookingBtnBorder { get; set; }

        public ICommand OnSearch { get; set; }
        public string SearchText { get; set; }
        public ICommand OnCurrentPosition { get; set; }

        // Book or Reserve Mode
        public Color BookModeBackColor { get; set; }
        public Color BookModeTextColor { get; set; }
        public ICommand OnBookMode { get; set; }
        public Color ReserveModeBackColor { get; set; }
        public Color ReserveModeTextColor { get; set; }
        public ICommand OnReserveMode { get; set; }
        public bool ReserveDatesVisible { get; set; }
        public string ResStartText { get; set; }
        public string ResEndText { get; set; }
        public ICommand OnReserveDatesTap { get; set; }
        public bool ReserveDatesPopupVisible { get; set; }
        public ICommand OnConfirmReserveDates { get; set; }
        public ICommand OnHideReserveDatesPopup { get; set; }
        private DateTime _resStartDate { get; set; }
        public DateTime ResStartDate 
        {
            get { return _resStartDate; }
            set { _resStartDate = value; OnResStartDateChanged(); } 
        }
        public TimeSpan ResStartTime { get; set; }
        public DateTime MinResStartDate { get; set; }
        public DateTime ResEndDate { get; set; }
        public TimeSpan ResEndTime { get; set; }
        public DateTime MinResEndDate { get; set; }

        public StackLayout MyMapContainer { get; set; }

        public bool ParkingPicturePopupVisible { get; set; }
        public ICommand OnParkingPictureTap { get; set; }
        public ICommand OnParkingPictureClose { get; set; }
        public string ParkingPicture { get; set; }

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IEventDataService _eventDataService;
        private readonly IOrderDataService _orderDataService;
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
        private static bool geolocationPermitted;
        private static bool checkingGeolocationPermission;

        // METHODS
        public HomeViewModel(IGeolocatorService geolocatorService,
                             IDialogService dialogService,
                             IApiService apiService,
                             IAuthService authService,
                             INavigationService navService,
                             IParkingDataService parkingDataService,
                             IEventDataService eventDataService,
                             IOrderDataService orderDataService,
                             IProfileService profileService)
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _eventDataService = eventDataService;
            _orderDataService = orderDataService;
            _authService = authService;
            _profileService = profileService;

            // Header actions
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);

            // Map actions
            OnSearch = new Command<object>(OnSearchMethod);
            OnCurrentPosition = new Command(OnCurrentPositionMethod);

            // Book now or Reserve choice
            OnBookMode = new Command(OnBookModeMethod);
            OnReserveMode = new Command(OnReserveModeMethod);
            OnReserveDatesTap = new Command(OnReserveDatesTapMethod);
            OnConfirmReserveDates = new Command(OnConfirmReserveDatesMethod);
            OnHideReserveDatesPopup = new Command(OnHideReserveDatesPopupMethod);
            OnBookModeChanged();

            InfoPanelVisible = false;
            connected = false;
            mapReady = false;
            geolocationPermitted = false;
            checkingGeolocationPermission = false;

            _profileService.Updating = false;

            // Zoomed parking picture popup
            ParkingPicturePopupVisible = false;
            OnParkingPictureTap = new Command<object>(OnParkingPictureTapMethod);
            OnParkingPictureClose = new Command(OnParkingPictureCloseMethod);
        }

        public override Task InitializeAsync(object data = null)
        {            
            // Map initialization
            if (_profileService.LastMapPosition != new Position(0, 0))
            {
                Map = new CustomMap(MapSpan.FromCenterAndRadius(new Position(_profileService.LastMapPosition.Latitude, _profileService.LastMapPosition.Longitude), Distance.FromKilometers(1)));
            }
            else
            {
                Map = new CustomMap();
            }
            Map.IsShowingUser = true;
            Map.MapType = MapType.Street;
            MyMapContainer.Children.Add(Map);

            Map.MapReady += Map_MapReady;
            Map.Tapped += Map_Tapped;
            Map.PinTapped += Map_PinTapped;
            Map.PropertyChanged += Map_PropertyChanged;
            Map.MapMoved += Map_MapMoved;

            // Set User data
            UserName = AuthSettings.User.Name;
            UserImage = "icon_no_user_256.png";
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserImage");
            base.OnPropertyChanged("UserMoney");

            InfoPanelVisible = false;
            base.OnPropertyChanged("InfoPanelVisible");

            return Task.FromResult(false);
        }

        public override async Task<bool> RefreshDataAsync()
        {            
            await GetParkingList();
            return await base.RefreshDataAsync();
        }

        void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!mapReady) return;

            CustomMap map = sender as CustomMap;
            if (e.PropertyName == "VisibleRegion") {
                _profileService.LastMapPosition = map.VisibleRegion.Center;
            }
        }

        void Map_MapMoved(object sender, MapMovedArgs e)
        {
            _profileService.LastMapPosition = e.Position;
        }

        public async void UpdateParkingListAsync()
        {
           await GetParkingList();
        }

        public async Task<bool> GetParkingList()
        {
            if (_profileService.Updating)
            {
                return await Task.FromResult(false);
            }

            // Check connection
            var result = ApiService.CheckConnection();
            if (result.IsSuccess == false) {
                _dialogService.ShowToast("Connessione ad internet assente", TimeSpan.FromSeconds(10));
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingListAsync(); return false; });                
                return await Task.FromResult(false);
            }

            connected = true;
            _profileService.Updating = true;

            // Check user login
            if ((!AuthService.IsUserAuthenticated()) && (!string.IsNullOrEmpty(AuthSettings.UserId)) && (!string.IsNullOrEmpty(AuthSettings.UserName))) {
                // Update User Data
                try
                {
                    var userResult = await _authService.GetUserByUserName(AuthSettings.UserName);
                    if (userResult.IsSuccess == false)
                    {
                        _dialogService.ShowToast("Manutenzione in corso", TimeSpan.FromSeconds(10));
                        Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingListAsync(); return false; });                        
                        return await Task.FromResult(false);
                    }
                    if (AuthService.IsUserAuthenticated())
                    {
                        // Update user data
                        UserName = AuthSettings.User.Name;
                        UserImage = "icon_no_user_256.png";
                        UserMoney = AuthSettings.UserCoin.ToString("N2");
                        base.OnPropertyChanged("UserName");
                        base.OnPropertyChanged("UserImage");
                        base.OnPropertyChanged("UserMoney");
                    }                    
                }
                catch (Exception e)
                {
                    _dialogService.ShowToast(e.Message, TimeSpan.FromSeconds(10));
                    Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingListAsync(); return false; });
                    _profileService.Updating = false;
                    return await Task.FromResult(false);
                }
                finally
                {
                    _profileService.Updating = false;
                }
            }

            _profileService.Updating = true;

            // Get Parking list
            try
            {
                var parkingsResult = await _parkingDataService.GetAllParkingsAsync();
                if ((parkingsResult == null) || (parkingsResult.Count == 0)) return await Task.FromResult(false);

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
                if ((eventsResult != null) && (eventsResult.Count != 0))
                {
                    foreach (EventModel availability in eventsResult)
                    {
                        UIParkingModel parkingModel = _profileService.GetParkingById(availability.ParkingId);
                        if (parkingModel != null)
                        {
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
                }

                RefreshMapPins();

                if (AuthService.IsUserAuthenticated())
                {
                    // Refresh user data, balance could be updated
                    await _profileService.RefreshUserData();
                    // Update user money if necessary
                    UserMoney = AuthSettings.UserCoin.ToString("N2");
                    base.OnPropertyChanged("UserMoney");
                }                
                return await Task.FromResult(false);
            }
            catch (Exception e) {
                _dialogService.ShowToast(e.Message, TimeSpan.FromSeconds(10));
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingListAsync(); return false; });
                return await Task.FromResult(false);
            } finally
            {
                _profileService.Updating = false;
            }
        }

        private void RefreshMapPins()
        {
            if ((mapReady == false) || (Map == null) || (Map.Pins == null)) {
                // Map not ready
                return;
            }

            Map.Pins.Clear();
            foreach (UIParkingModel parking in _profileService.ParkingList)
            {
                // Add Map Pin
                CreatePin(new Position(parking.Latitude, parking.Longitude), parking);
            }
        }

        private void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Position, Distance.FromKilometers(1)));
        }

        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android) {
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(e.Parking.Latitude, e.Parking.Longitude), Distance.FromKilometers(1)));
            }

            UIParkingModel parking = _profileService.GetParkingById(e.Parking.Id);
            if (parking != null) {
                // Parking found

                // Get parking status
                bool isFree = false;
                if (_profileService.UserReserveMode)
                {
                    // Remove seconds from times
                    ResStartTime = ResStartTime.Subtract(TimeSpan.FromSeconds(ResStartTime.Seconds));
                    ResEndTime = ResEndTime.Subtract(TimeSpan.FromSeconds(ResEndTime.Seconds));
                    isFree = parking.isFree(ResStartDate + ResStartTime, ResEndDate + ResEndTime);
                    BookingBtnText = "Prenota";
                    BookingBtnEnable = true;
                    BookingBtnBorder = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"];
                    BookingBtnBackground = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"];
                }
                else
                {
                    isFree = parking.isFree();
                    if (isFree) {
                        BookingBtnEnable = true;
                        BookingBtnBorder = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"];
                        BookingBtnBackground = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"];
                    } else {
                        BookingBtnEnable = false;
                        BookingBtnBorder = Color.Gray;
                        BookingBtnBackground = Color.FromHex("#E3E3E3");
                    }
                    BookingBtnText = "Occupa";
                }

                // Update Info panel data
                if (string.IsNullOrEmpty(parking.ImageUrl))
                {
                    Picture = "icon_no_photo.png";
                }
                else
                {
                    Picture = ApiSettings.BaseUrl + parking.ImageUrl;
                }
                StatusText = (isFree) ? "Disponibile" : "Occupato";
                PriceText = parking.PriceMin.ToString("N2") + " CHF/h";
                UID = parking.Id;

                InfoPanelVisible = true;

                base.OnPropertyChanged("Picture");
                base.OnPropertyChanged("StatusText");
                base.OnPropertyChanged("PriceText");
                base.OnPropertyChanged("UID");
                base.OnPropertyChanged("BookingBtnText");
                base.OnPropertyChanged("BookingBtnBorder");
                base.OnPropertyChanged("BookingBtnBackground");
                base.OnPropertyChanged("BookingBtnEnable");
                base.OnPropertyChanged("InfoPanelVisible");
            }
        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {
            Map_Ready_Handler();
        }

        private async void Map_Ready_Handler()
        {
            // Xamarin.Forms bug on Android, Map_Ready is called twice
            if (mapReady) return;

            mapReady = true;

            if (_profileService.LastMapPosition == new Position(0, 0))
            {
                MoveToCurrentPosition();
            } else
            {
                geolocationPermitted = await _geoLocatorService.IsPermissionGaranted();
                if (geolocationPermitted)
                {
                    Map.ShowUserEnable = true;
                }
            }
            await GetParkingList();            
        }

        private void CreatePin(Position position, UIParkingModel parking)
        {
            // Get parking status
            bool isFree = false;
            if (_profileService.UserReserveMode) {
                // Remove seconds from times
                ResStartTime = ResStartTime.Subtract(TimeSpan.FromSeconds(ResStartTime.Seconds));
                ResEndTime = ResEndTime.Subtract(TimeSpan.FromSeconds(ResEndTime.Seconds));
                isFree = parking.isFree(ResStartDate + ResStartTime, ResEndDate + ResEndTime);
            } else {
                isFree = parking.isFree();
            }

            // Create new custom pin
            var pin = new CustomPin
            {
                Id = parking.Id,
                Parking = parking,
                Type = PinType.Place,
                Position = position,
                Label = parking.Address,
                Address = parking.Cap.ToString() + " " + parking.City,
                Icon = (isFree) ? "icon_pin_green_256" : "icon_pin_red_256"
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
                NavigationService.NavigateToAsync<MoneyViewModel>();
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

        // Booking Tap action
        public void OnBookingTappedMethod(object id)
        {
            if (_authService.IsUserAuthenticated())
            {
                if (_profileService.UserReserveMode)
                {
                    // Reservation
                    UIBookingModel booking = new UIBookingModel
                    {
                        StartDate = ResStartDate + ResStartTime,
                        EndDate = ResEndDate + ResEndTime,
                        ParkingId = (int)id
                    };
                    if (booking != null)
                    {
                        NavigationService.NavigateToAsync<ReservationViewModel>(booking);
                    }
                }
                else
                {
                    UIParkingModel parking = _profileService.GetParkingById((int)id);
                    if (parking != null)
                    {
                        NavigationService.NavigateToAsync<BookingViewModel>(parking);
                    }
                }
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
                var geoLocation = await _geoLocatorService.GetLocation();

                if (geoLocation == null) {
                    // Check if dialog message has to be shown
                    geolocationPermitted = await _geoLocatorService.IsPermissionGaranted();
                    if (geolocationPermitted == false)
                    {
                        await _dialogService.ShowAlert("Avviso", "Abilita i servizi di geolocalizzazione e autorizza NextPark ad averne accesso per localizzarti sulla mappa");
                    }
                    // Wait for position ready
                    geolocationPermitted = false;
                    if (checkingGeolocationPermission == false)
                    {
                        checkingGeolocationPermission = true;
                        Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(3), () => { CheckGeolocationPermission(); return false; });
                    }
                    return;
                }

                _profileService.LastMapPosition = geoLocation.ToXamMapPosition();

                Map.ShowUserEnable = true;
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(_profileService.LastMapPosition, Distance.FromKilometers(1)));                
            } catch (Exception) { }                             
        }

        public void OnReserveModeMethod()
        {
            _profileService.UserReserveMode = true;
            OnBookModeChanged();
        }

        public void OnBookModeMethod()
        {
            _profileService.UserReserveMode = false;
            OnBookModeChanged();
        }

        public void OnBookModeChanged()
        {
            if (_profileService.UserReserveMode) {
                BookModeBackColor = Color.White;
                BookModeTextColor = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"]; //Color.Gray;
                ReserveModeBackColor = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"]; //Color.Gray;
                ReserveModeTextColor = Color.White;
                ReserveDatesVisible = true;
                MinResStartDate = DateTime.Now.Date;
                MinResEndDate = MinResStartDate;
                base.OnPropertyChanged("MinResStartDate");
                base.OnPropertyChanged("MinResEndDate");
                if (_profileService.UserStartDate < DateTime.Now.Date) {
                    _profileService.UserStartDate = DateTime.Now;
                    _profileService.UserEndDate = DateTime.Now.AddHours(1);
                }
                ResStartDate = _profileService.UserStartDate.Date;
                ResStartTime = _profileService.UserStartDate.TimeOfDay;
                ResEndDate = _profileService.UserEndDate.Date;
                ResEndTime = _profileService.UserEndDate.TimeOfDay;
                base.OnPropertyChanged("ResStartDate");
                base.OnPropertyChanged("ResStartTime");
                base.OnPropertyChanged("ResEndDate");
                base.OnPropertyChanged("ResEndTime");

                ResStartText = ResStartDate.ToString("ddd d MMM ") + ResStartTime.ToString(@"hh\:mm");
                ResEndText = ResEndDate.ToString("ddd d MMM ") + ResEndTime.ToString(@"hh\:mm");
                base.OnPropertyChanged("ResStartText");
                base.OnPropertyChanged("ResEndText");
            } else {
                BookModeBackColor = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"]; //Color.Gray;
                BookModeTextColor = Color.White;
                ReserveModeBackColor = Color.White;
                ReserveModeTextColor = (Color)Xamarin.Forms.Application.Current.Resources["NextParkColor1"];//Color.Gray;
                ReserveDatesVisible = false;
                ReserveDatesPopupVisible = false;
                base.OnPropertyChanged("ReserveDatesPopupVisible");
            }

            // Hide current parking information panel
            InfoPanelVisible = false;
            base.OnPropertyChanged("InfoPanelVisible");

            base.OnPropertyChanged("BookModeBackColor");
            base.OnPropertyChanged("BookModeTextColor");
            base.OnPropertyChanged("ReserveModeBackColor");
            base.OnPropertyChanged("ReserveModeTextColor");
            base.OnPropertyChanged("ReserveDatesVisible");

            RefreshMapPins();
        }

        public void OnReserveDatesTapMethod()
        {
            ReserveDatesPopupVisible = true;
            base.OnPropertyChanged("ReserveDatesPopupVisible");
        }

        public void OnConfirmReserveDatesMethod()
        {
            ReserveDatesPopupVisible = false;
            base.OnPropertyChanged("ReserveDatesPopupVisible");
            ResStartText = ResStartDate.ToString("ddd d MMM ") + ResStartTime.ToString(@"hh\:mm");
            ResEndText = ResEndDate.ToString("ddd d MMM ") + ResEndTime.ToString(@"hh\:mm");
            base.OnPropertyChanged("ResStartText");
            base.OnPropertyChanged("ResEndText");
            _profileService.UserStartDate = ResStartDate + ResStartTime;
            _profileService.UserEndDate = ResEndDate + ResEndTime;

            RefreshMapPins();
        }

        public void OnHideReserveDatesPopupMethod()
        {
            ReserveDatesPopupVisible = false;
            base.OnPropertyChanged("ReserveDatesPopupVisible");
        }

        private void OnResStartDateChanged()
        {
            MinResEndDate = _resStartDate;
            if (ResEndDate < _resStartDate)
            {
                ResEndDate = _resStartDate;
                base.OnPropertyChanged("ResEndDate");
            }
            base.OnPropertyChanged("MinResEndDate");
        }

        private async void CheckGeolocationPermission()
        {
            if (geolocationPermitted == false)
            {
                if (NavigationService.CurrentPage is Views.HomePage)
                {
                    try
                    {
                        geolocationPermitted = await _geoLocatorService.IsPermissionGaranted();
                        if (geolocationPermitted == false)
                        {
                            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(3), () => { CheckGeolocationPermission(); return false; });
                            return;
                        }

                        var geoLocation = await _geoLocatorService.GetLocation();

                        if (geoLocation == null) return;

                        _profileService.LastMapPosition = geoLocation.ToXamMapPosition();

                        Map.ShowUserEnable = true;
                        Map.MoveToRegion(MapSpan.FromCenterAndRadius(_profileService.LastMapPosition, Distance.FromKilometers(1)));                        
                    }
                    catch (Exception) { return; }
                }
            }            
            checkingGeolocationPermission = false;
        }

        
        public void OnParkingPictureTapMethod(object id)
        {
            if (InfoPanelVisible == true)
            {
                UIParkingModel parking = _profileService.GetParkingById((int)id);
            
                if ((parking != null) && (!String.IsNullOrEmpty(parking.ImageUrl))) {
                    ParkingPicturePopupVisible = true;
                    ParkingPicture = ApiSettings.BaseUrl + parking.ImageUrl;
                    base.OnPropertyChanged("ParkingPicture");
                    base.OnPropertyChanged("ParkingPicturePopupVisible");
                }
            }
        }

        public void OnParkingPictureCloseMethod()
        {
            ParkingPicturePopupVisible = false;
            base.OnPropertyChanged("ParkingPicturePopupVisible");
        }
    }
}
