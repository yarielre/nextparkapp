using System;
using System.Threading.Tasks;
using NextPark.Mobile.Settings;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class LaunchScreenViewModel : BaseViewModel
    {
        // PROPERTIES
        public bool IsRunning { get; set; }         // Activity spinner

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IPushService _pushService;

        // METHODS
        public LaunchScreenViewModel(IDialogService dialogService,
                                     IApiService apiService,
                                     IAuthService authService,
                                     IParkingDataService parkingDataService,
                                     INavigationService navService,
                                     IPushService pushService)
        : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _pushService = pushService;

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
            await AutoLogin();

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

        public async Task<bool> AutoLogin()
        {
            if ((AuthSettings.UserId != null) && (AuthSettings.UserName != null))
            {
                try
                {
                    var userResponse = await AuthService.GetUserByUserName(AuthSettings.UserName);

                    // Check user data response
                    if (userResponse.IsSuccess == true)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception e) { return false; }
            }
            return false;
        }
    }
}
