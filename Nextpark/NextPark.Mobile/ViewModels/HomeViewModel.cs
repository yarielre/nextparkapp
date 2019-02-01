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
using NextPark.Mobile.Core.Settings;
using NextPark.Enums.Enums;
using NextPark.Mobile.CustomControls;

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

        private CustomControls.CustomMap Map { get; set; }  // Custom Map
        public ICommand OnBookingTapped { get; set; }       // Booking button click action

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;
        private readonly ParkingDataService _parkingDataService;
        private readonly EventDataService _eventDataService;

        private readonly IAuthService _authService;

        // PRIVATE VARIABLES
        private ObservableCollection<ParkingInfo> parkings;
        public ObservableCollection<ParkingInfo> Parkings
        {
            get { return parkings; }

            //set => SetValue(ref parkings, value); TODO: Use this.
            set { parkings = value; base.OnPropertyChanged("Parkings"); }
        }

        // METHODS
        public HomeViewModel(IGeolocatorService geolocatorService, IDialogService dialogService,
            IApiService apiService, IAuthService authService, INavigationService navService,
            ParkingDataService parkingDataService, EventDataService eventDataService)
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _eventDataService = eventDataService;
            _authService = authService;

            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);
        }

        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }
            if (data is CustomControls.CustomMap map)
            {
                Map = map;
                Map.MapReady += Map_MapReady;
                Map.Tapped += Map_Tapped;
                Map.PinTapped += Map_PinTapped;
            }

            // Set User data
            UserName = AuthSettings.UserName;
            UserImage = "icon_no_user_256.png";
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserImage");
            base.OnPropertyChanged("UserMoney");

            // TODO: fill parking list and use parkingModel
            Parkings = new ObservableCollection<ParkingInfo>
            {
                new ParkingInfo {
                UID = 0,
                Info = "Via Strada 1",
                SubInfo = "Lugano, Ticino",
                Picture="image_parking1.png",
                FullPrice = "2 CHF/h",
                FullAvailability = "08:00-12:00",
                BookAction = OnBookingTapped},

                new ParkingInfo { UID = 1, Info = "Via Strada 1.5", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped},
                new ParkingInfo { UID = 2, Info = "Via Strada 2", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped}
            };

            //DemoBackEndCalls();

            UpdateParkingList();

            //base.OnPropertyChanged("Parkings");

            return Task.FromResult(false);
        }

        private async void DemoBackEndCalls()
        {

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
        }

        public async void UpdateParkingList()
        {
            await GetParkings();
        }
        private async Task GetParkings()
        {

            //Demo Login OK
            //var loginResponse = await AuthService.Login("JarJar", "Jaro.001");
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
                        UID = parking.Id,
                        Info = parking.Address,
                        SubInfo = parking.Cap.ToString() + " " + parking.City,
                        FullAvailability = (parking.Status == ParkingStatus.Enabled) ? "disponibile" : "occupato",
                        FullPrice = parking.PriceMin.ToString() + " CHF/h",
                        BookAction = OnBookingTapped
                    });
                }


                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    UpdateMapPins();
                    return false;
                });

            } catch (Exception e) {
                await _dialogService.ShowErrorAlert(e.Message);
            }




            //base.OnPropertyChanged("Parkings");

            /*
            //Demo Post Parking Working on It!
            var parking1 = new ParkingModel
            {
                ImageUrl = "image_parking1.png",
                IsRented = false,
                ParkingEvent = new EventModel
                {
                    EndDate = DateTime.Now,
                    StartDate = DateTime.Now
                },
                ParkingCategory = new ParkingCategoryModel
                {
                    Category = "Test",
                    HourPrice = 2.0,
                    MonthPrice = 3.0
                },
                ParkingType = new ParkingTypeModel
                {
                    Type = "Business"
                }
            };

            var response = await _parkingDataService.Post(parking1);

            // TODO: fill parking list and use parkingModel
            Parkings = new ObservableCollection<ParkingInfo>
            {
                new ParkingInfo {
                UID = 0,
                Info = "Via Strada 1",
                SubInfo = "Lugano, Ticino",
                Picture="image_parking1.png",
                FullPrice = "2 CHF/h",
                FullAvailability = "08:00-12:00",
                BookAction = OnBookingTapped},

                new ParkingInfo { UID = 1, Info = "Via Strada 1.5", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped},
                new ParkingInfo { UID = 2, Info = "Via Strada 2", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped}
            };
            */
        }

        private void UpdateMapPins()
        {
            Map_Ready_Handler();

            /*
            // Update parkings
            if (_parkingDataService.Parkings.Count > 0)
            {
                foreach (ParkingModel parking in _parkingDataService.Parkings)
                {
                    var pin = new CustomPin
                    {
                        Id = parking.Id,
                        Parking = parking,
                        Type = PinType.Place,
                        Position = new Position(parking.Latitude, parking.Longitude),
                        Label = parking.Address,
                        Address = parking.Cap.ToString() + " " + parking.City,
                        Icon = "ic_location_green"
                    };
                    if (parking.Status == ParkingStatus.Disabled)
                    {
                        pin.Icon = "ic_location_black";
                    }
                    Map.Pins.Add(pin);
                }
                base.OnPropertyChanged("Parkings");
            }
            */
        }

        private async void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            //throw new System.NotImplementedException();
            // DEMO ONLY!
            if (_authService.IsUserAuthenticated()) {
                ParkingInfo item = Parkings[0];
                await NavigationService.NavigateToAsync<BookingViewModel>(item);
            }
            _dialogService.ShowAlert("Map Tapped", string.Format("Lat {0}, Long {1}", e.Position.Latitude, e.Position.Longitude));

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Position, Distance.FromKilometers(1)));

            var demoParking = new ParkingModel
            {
                Id = 1,
                Latitude = e.Position.Latitude,
                Longitude = e.Position.Longitude,

            };

            CreatePin(e.Position, demoParking);
        }

        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            _dialogService.ShowConfirmAlert("Pin Tapped", "Marker tapped");

        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {

            Map_Ready_Handler();
        }

        private void CallMapReady()
        {
            Map_Ready_Handler();
        }

        private async void Map_Ready_Handler()
        {

            Xamarin.Forms.Maps.Position position = new Position(0, 0);
            try
            {

                var geoLocation = await _geoLocatorService.GetLocation();

                if (geoLocation == null) return;

                position = geoLocation.ToXamMapPosition();

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));


            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));

            /*
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                // Update parkings
                if (_parkingDataService.Parkings.Count > 0)
                {
                    foreach (ParkingModel parking in _parkingDataService.Parkings)
                    {
                        Map.Pins.Add(new Pin
                        {
                            Position = new Position(parking.Latitude, parking.Longitude),
                            Label = parking.Address,
                            BindingContext = Map.BindingContext
                        });
                    }
                    base.OnPropertyChanged("Parkings");
                }
                return false;
            });
            */


        }
        private void CreatePin(Position position, ParkingModel parking)
        {
            var pin = new CustomPin
            {
                Id = parking.Id,
                Parking = parking,
                Type = PinType.Place,
                Position = position,
                Label = "custom pin",
                Address = "custom detail info",
                Icon = "icon_coins_48"
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
            else
            {
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
    }
}
