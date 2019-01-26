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
using NextPark.Mobile.Core.Settings;

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
            IApiService apiService, IAuthService authService, INavigationService navService, ParkingDataService parkingDataService)
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
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
            //GetParkings(); //TODO: Use this!


            base.OnPropertyChanged("Parkings");

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
                var parkings = await _parkingDataService.Get();

             

                if (parkings.Count > 0) return;

                //Demo Posting Parking
                var parking1 = new ParkingModel
                {
                    Address = "Via Strada",
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


                var postedParking = await _parkingDataService.Post(parking1);


                postedParking.Status = Enums.Enums.ParkingStatus.Disabled;

                //Demo Puting Parking
                var parkingResult =  await _parkingDataService.Put(postedParking, postedParking.Id);

                //Demo Deleting Parking
                var deletedParking =  await _parkingDataService.Delete(parkingResult.Id);


            }
            catch (Exception e)
            {

            }
        }


        private async Task GetParkings()
        {

            //Demo Login OK
            var loginResponse = await AuthService.Login("JarJar", "Jaro.001");

            var parkingsResponse = await _parkingDataService.Get();

            if (parkingsResponse.Count > 0) return;

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
        }

        private void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            //throw new System.NotImplementedException();
            // DEMO ONLY! 
            if (_authService.IsUserAuthenticated()) {
                ParkingInfo item = Parkings[0];
                NavigationService.NavigateToAsync<BookingViewModel>(item);
            }
        }

        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {

            Map_Ready_Handler();
        }

        private async void Map_Ready_Handler()
        {

            Xamarin.Forms.Maps.Position position = new Position(0, 0);
            try
            {
                var permissionGaranted = await _geoLocatorService.IsPermissionGaranted();

                if (!permissionGaranted) return;

                var getLocation = await _geoLocatorService.GetLocation();

                position = getLocation.ToXamMapPosition();
            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));

            // Update parkings
            if (Parkings.Count > 0)
            {
                base.OnPropertyChanged("Parkings");
            }

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
