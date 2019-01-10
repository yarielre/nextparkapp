using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;

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
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        private CustomControls.CustomMap Map { get; set; }  // Custom Map
        public ICommand OnBookingTapped { get; set; }       // Booking button click action

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private ObservableCollection<ParkingInfo> parkings;
        public ObservableCollection<ParkingInfo> Parkings
        {
            get { return parkings; }
            set { parkings = value; base.OnPropertyChanged("Parkings"); }
        }

        // METHODS
        public HomeViewModel(IGeolocatorService geolocatorService, IDialogService dialogService, 
            IApiService apiService,  IAuthService authService, INavigationService navService) 
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;

            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);

            UserName = "Accedi";
            UserMoney = "0";
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

            UserName = "Jonny";
            UserMoney = "8";
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // TODO: fill parking list
            Parkings = new ObservableCollection<ParkingInfo>
            {
                new ParkingInfo { UID = 0, Info = "Via Strada 1", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped},
                new ParkingInfo { UID = 1, Info = "Via Strada 1.5", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped},
                new ParkingInfo { UID = 2, Info = "Via Strada 2", SubInfo = "Lugano, Ticino", Picture="image_parking1.png", FullPrice = "2 CHF/h", FullAvailability = "08:00-12:00", BookAction = OnBookingTapped}
            };
            base.OnPropertyChanged("Parkings");

            return Task.FromResult(false);
        }

        private void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {

            Map_Ready_Handler();
        }

        private async void Map_Ready_Handler() {

            Xamarin.Forms.Maps.Position position = new Position(0,0);
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
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<RegisterViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Booking Tap action
        public void OnBookingTappedMethod(object args)
        {
            if (args is int)
            {
                ParkingInfo item = Parkings[(int)args];
                NavigationService.NavigateToAsync<BookingViewModel>(item);
                //_dialogService.ShowAlert("Alert", "Booking: " + args.ToString());
            }
        }
    }
}
