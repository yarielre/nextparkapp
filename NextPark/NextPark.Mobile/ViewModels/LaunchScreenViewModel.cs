using System;
using System.Threading.Tasks;
using NextPark.Mobile.Core.Settings;
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
        private readonly ParkingDataService _parkingDataService;

        // METHODS
        public LaunchScreenViewModel(IDialogService dialogService,
                                     IApiService apiService,
                                     IAuthService authService,
                                     ParkingDataService parkingDataService,
                                     INavigationService navService)
        : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;

            IsRunning = false;
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            // Get parking list
            //GetParkingList();

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

            // Read parking list and store it into a singleton
            await NavigationService.NavigateToAsync<HomeViewModel>();
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
