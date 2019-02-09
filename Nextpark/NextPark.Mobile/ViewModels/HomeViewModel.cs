﻿using NextPark.Mobile.Extensions;
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
using NextPark.Mobile.Core.Settings;
using NextPark.Enums.Enums;
using NextPark.Mobile.CustomControls;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.ViewModels
{
    public class ParkingInfo
    {
        public int UID { get; set; }
        public string Info { get; set; }
        public string SubInfo { get; set; }
        public string Picture { get; set; }
        public string FullPrice { get; set; }
        public string FullAvailability { get; set; }
        public ICommand BookAction { get; set; }
    }

    public class HomeViewModel : BaseViewModel
    {
        // PROPERTIES
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public ImageSource UserImage { get; set; }  // Header user image
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Info { get; set; }
        public string SubInfo { get; set; }
        public string Picture { get; set; }
        public int Index { get; set; }
        public bool InfoPanelVisible { get; set; }  // Parking Info Panel visibility

        public CustomControls.CustomMap Map { get; set; }  // Custom Map
        public ICommand OnBookingTapped { get; set; }       // Booking button click action

        public ICommand OnSearch { get; set; }
        public string SearchText { get; set; }
        public ICommand OnCurrentPosition { get; set; }

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;
        private readonly ParkingDataService _parkingDataService;
        private readonly EventDataService _eventDataService;
        private readonly InAppPurchaseService _inAppPurchaseService;
        private readonly IProfileService _profileService;

        private readonly IAuthService _authService;

        // PRIVATE VARIABLES
        private ObservableCollection<ParkingInfo> parkings;
        public ObservableCollection<ParkingInfo> Parkings
        {
            get { return parkings; }

            //set => SetValue(ref parkings, value); TODO: Use this.
            set { parkings = value; base.OnPropertyChanged("Parkings"); }
        }
        private static bool connected;

        // METHODS
        public HomeViewModel(IGeolocatorService geolocatorService, 
                             IDialogService dialogService,
                             IApiService apiService, 
                             IAuthService authService, 
                             INavigationService navService,
                             ParkingDataService parkingDataService, 
                             EventDataService eventDataService, 
                             InAppPurchaseService inAppPurchaseService, 
                             IProfileService profileService)
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _eventDataService = eventDataService;
            _authService = authService;
            _inAppPurchaseService = inAppPurchaseService;
            _profileService = profileService;

            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);
            OnSearch = new Command<object>(OnSearchMethod);
            OnCurrentPosition = new Command(OnCurrentPositionMethod);

            InfoPanelVisible = false;

            Parkings = new ObservableCollection<ParkingInfo>();

            connected = false;

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

            if (Parkings != null) {
                Parkings.Clear();
            }

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) {
                Map_Ready_Handler();
            }

            //DemoBackEndCalls();

            UpdateParkingList();

            return Task.FromResult(false);
        }

        void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CustomMap map = sender as CustomMap;
            if (e.PropertyName == "VisibleRegion") {
                _profileService.LastUserPosition = map.VisibleRegion.Center;
            }
        }


        private async void DemoBackEndCalls()
        {
            /*
            try
            {
                //Demo Login OK
                var loginResponse = await AuthService.Login("demo@nextpark.ch", "Wisegar.1");

                //Demo Register OK
                var demoUser = new RegisterModel
                {
                    Address = "Via Demo User",
                    CarPlate = "TI 00DEMO00",
                    Email = "demo@nextpark.ch",
                    Lastname = "Demo",
                    Name = "User",
                    Password = "Wisegar.1",
                    State = "DemoState",
                    Username = "demo@nextpark.ch"
                };

                var registerResponse = await AuthService.Register(demoUser);

                //Demo Get Parkings OK
                var parkingList = await _parkingDataService.Get();


                if (parkings.Count > 0) {
                    await _parkingDataService.Delete(parkings[0].Id);
                }

                //Demo Posting Parking
                var parking1 = new ParkingModel
                {
                    Address = "Via Strada",
                    Cap = 7777,
                    City = "Lugano",
                    CarPlate = "TI 000000",
                    Latitude = 40,
                    Longitude = 40,
                    PriceMax = 4,
                    PriceMin = 4,
                    State = "Ticino",
                    Status = Enums.Enums.ParkingStatus.Enabled,
                    UserId = 1,
                    ImageUrl = "image_parking1.png"
                };

                //Demo Posting Parking
                var postedParking = await _parkingDataService.Post(parking1);

                var eventParking = new EventModel
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    ParkingId = postedParking.Id,
                    RepetitionEndDate = DateTime.Now,
                    RepetitionType = Enums.Enums.RepetitionType.Dayly,
                    MonthRepeat = new List<Enums.MyMonthOfYear>(),
                    WeekRepeat = new List<MyDayOfWeek>()

                };
                postedParking.Status = Enums.Enums.ParkingStatus.Disabled;

                var result = await _eventDataService.Post(eventParking);

                //Demo Puting Parking
                var parkingResult = await _parkingDataService.Put(postedParking, postedParking.Id);

                //Demo Deleting Parking
                var deletedParking = await _parkingDataService.Delete(parkingResult.Id);

            }
            catch (Exception e)
            {

            }
            */
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
            if ((!AuthService.IsUserAuthenticated()) && (AuthSettings.UserId != null) && (AuthSettings.UserName != null)) {
                // Update User Data
                try {
                    var userResult = await _authService.GetUserByUserName(AuthSettings.UserName);
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
                var parkingsResponse = await _parkingDataService.Get();
                _parkingDataService.Parkings = parkingsResponse;

                if (parkingsResponse.Count == 0) return;

                Parkings.Clear();

                foreach (ParkingModel parking in parkingsResponse)
                {
                    Parkings.Add(new ParkingInfo
                    {
                        Picture = "image_parking1.png",
                        UID = parking.Id,
                        Info = parking.Address,
                        SubInfo = parking.Cap.ToString() + " " + parking.City,
                        FullAvailability = (parking.Status == ParkingStatus.Enabled) ? "disponibile" : "occupato",
                        FullPrice = parking.PriceMin.ToString() + " CHF/h",
                        BookAction = OnBookingTapped
                    });
                    CreatePin(new Position(parking.Latitude, parking.Longitude), parking);
                }
                base.OnPropertyChanged("Parkings");
            }
            catch (Exception e) {
                _dialogService.ShowToast(e.Message, TimeSpan.FromSeconds(10));
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(9), () => { UpdateParkingList(); return false; });
            }
        }

        private void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            //await _dialogService.ShowAlert("Map Tapped", string.Format("Lat {0}, Long {1}", e.Position.Latitude, e.Position.Longitude));

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Position, Distance.FromKilometers(1)));

            /*
            var demoParking = new ParkingModel
            {
                Id = 1,
                Latitude = e.Position.Latitude,
                Longitude = e.Position.Longitude,
                Address = "Custom Pin",
                Cap = 0,
                City = "City"

            };

            CreatePin(e.Position, demoParking);

            TestPaymentAsync();
            */
        }
        private async Task TestPaymentAsync() {

            var result = await _dialogService.ShowConfirmAlert("Map Tapped", "Test the payment?");

            if (result) {
               var purchaseResult = this._inAppPurchaseService.MakePurchase();


            }
        }
        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            int index = 0;
            foreach (ParkingInfo parking in Parkings) {
                if (parking.UID == e.Parking.Id) {
                    Index = index;
                    break;
                }
                index++;
            }
            Picture = e.Parking.ImageUrl;
            if ((Picture == null) || (Picture.Equals(""))) 
            {
                Picture = "icon_no_photo.png";
            }
            Info = (e.Parking.Status == ParkingStatus.Enabled) ? "Disponibile" : "Occupato";
            SubInfo = e.Parking.PriceMin + " CHF/h";
            InfoPanelVisible = true;

            base.OnPropertyChanged("Picture");
            base.OnPropertyChanged("Info");
            base.OnPropertyChanged("SubInfo");
            base.OnPropertyChanged("Index");
            base.OnPropertyChanged("InfoPanelVisible");
        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {

            Map_Ready_Handler();
        }

        private async void Map_Ready_Handler()
        {        
            try
            {
                if ((_profileService.LastUserPosition == null) || (_profileService.LastUserPosition == new Position(0,0)))
                {
                    _profileService.LastUserPosition = new Position(0, 0);

                    var geoLocation = await _geoLocatorService.GetLocation();

                    if (geoLocation == null) return;

                    _profileService.LastUserPosition = geoLocation.ToXamMapPosition();
                }

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(_profileService.LastUserPosition, Distance.FromKilometers(1)));

            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }
        }

        private void CreatePin(Position position, ParkingModel parking)
        {
            var pin = new CustomPin
            {
                Id = parking.Id,
                Parking = parking,
                Type = PinType.Place,
                Position = position,
                Label = parking.Address,
                Address = parking.Cap.ToString() + " " + parking.City,
                Icon = (parking.Status == ParkingStatus.Enabled) ? "icon_pin_green_256" : "icon_pin_red_256"
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
        public void OnBookingTappedMethod(object args)
        {
            if (_authService.IsUserAuthenticated())
            {
                if (args is int)
                {
                    ParkingInfo item = Parkings[(int)args];
                    NavigationService.NavigateToAsync<BookingViewModel>(item);
                    //_dialogService.ShowAlert("Alert", "Booking: " + args.ToString());
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
                _profileService.LastUserPosition = new Position(0, 0);

                var geoLocation = await _geoLocatorService.GetLocation();

                if (geoLocation == null) return;

                _profileService.LastUserPosition = geoLocation.ToXamMapPosition();

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(_profileService.LastUserPosition, Distance.FromKilometers(1)));

            } catch (Exception e) {}
        }
    }
}
