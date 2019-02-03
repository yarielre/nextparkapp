using System;
using System.Threading.Tasks;
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

        public void GetParkingList()
        {
            // Read parking list and store it into a singleton
            NavigationService.NavigateToAsync<HomeViewModel>();
        }
    }
}
