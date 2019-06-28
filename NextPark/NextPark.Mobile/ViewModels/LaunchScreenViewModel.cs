using System;
using System.Threading.Tasks;
using NextPark.Mobile.Settings;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using Xamarin.Forms;
using NextPark.Mobile.Extensions;

namespace NextPark.Mobile.ViewModels
{
    public class LaunchScreenViewModel : BaseViewModel
    {
        // PROPERTIES
        public bool IsRunning { get; set; }         // Activity spinner

        // SERVICES
        private readonly IProfileService _profileService;
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IPushService _pushService;
        private readonly IGeolocatorService _geoLocatorService;

        // METHODS
        public LaunchScreenViewModel(IDialogService dialogService,
                                     IApiService apiService,
                                     IAuthService authService,
                                     IProfileService profileService,
                                     IParkingDataService parkingDataService,
                                     INavigationService navService,
                                     IPushService pushService,
                                     IGeolocatorService geolocatorService)
        : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _pushService = pushService;
            _profileService = profileService;
            _geoLocatorService = geolocatorService;

            IsRunning = false;
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {        
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Get parking list
            Device.StartTimer(TimeSpan.FromSeconds(1), () => { GetParkingList(); return false; });

            return Task.FromResult(false);
        }

        public async void GetParkingList()
        {
            // Check autologin
            await _profileService.RefreshUserData();

            await GetCurrentLocation();

            // Stop activity spinner
            IsRunning = false;
            base.OnPropertyChanged("IsRunning");

            /*
             * CODE to go directly on order page if push of an expired order arrives when app were closed
             * 
            bool pushServiceChangePage = false;

            if (PushSettings.NewNotification) {
                pushServiceChangePage = await _pushService.NotificationHandler();
            }
            if (!pushServiceChangePage)
            {
                await NavigationService.NavigateToAsync<HomeViewModel>();
            }
            */            

            await NavigationService.NavigateToAsync<HomeViewModel>();

            _pushService.Start();
        }

        public async Task GetCurrentLocation()
        {
            try
            {
                var geoLocation = await _geoLocatorService.GetLocation();

                if (geoLocation != null)
                {
                    _profileService.LastMapPosition = geoLocation.ToXamMapPosition();
                }
            }
            catch (Exception) { }
        }
    }
}
