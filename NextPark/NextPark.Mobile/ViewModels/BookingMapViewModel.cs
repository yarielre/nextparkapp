using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using NextPark.Mobile.Controls;

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;
using NextPark.Mobile.Settings;

namespace NextPark.Mobile.ViewModels
{
    public class BookingMapParams
    {
        public CustomControls.CustomMap Map { get; set; }
        public BookingItem Booking { get; set; }
    }

    public class BookingMapViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        private CustomControls.CustomMap Map { get; set; }  // Custom Map
        public ICommand OnArrived { get; set; }             // Arrived button click action
        public ICommand OnNavigate { get; set; }            // Navigate button click action
        public ICommand OnBookDel { get; set; }             // Delete button click action

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;

        // METHODS
        public BookingMapViewModel(IGeolocatorService geolocatorService, 
                                   IDialogService dialogService,
                                   IApiService apiService, 
                                   IAuthService authService, 
                                   INavigationService navService)
                                   : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnArrived = new Command<object>(OnArrivedMethod);
            OnNavigate = new Command<object>(OnNavigateMethod);
            OnBookDel = new Command<object>(OnBookDelMethod);
        }

        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }
            if (data is BookingMapParams parameter)
            {
                Map = parameter.Map;
                Map.MapReady += Map_MapReady;
                Map.Tapped += Map_Tapped;
                Map.PinTapped += Map_PinTapped;
            }

            // Header
            BackText = "Indietro";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

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
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserBookingViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Arrived button click action
        public void OnArrivedMethod(object sender)
        {
            // TODO: manage user arrived
            _dialogService.ShowAlert("Alert", "TODO: manage user arrived");
        }

        // Navigate button click action
        public void OnNavigateMethod(object sender)
        {
            // TODO: manage navigation to parking
            _dialogService.ShowAlert("Alert", "TODO: manage navigation to parking");
        }

        // Delete button click action
        public void OnBookDelMethod(object sender)
        {
            // TODO: manage booking delete action
            _dialogService.ShowAlert("Alert", "TODO: manage booking delete action");
        }
    }
}
