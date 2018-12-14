using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private CustomControls.CustomMap Map { get; set; }
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;

        public HomeViewModel(IGeolocatorService geolocatorService, IDialogService dialogService, 
            IApiService apiService,  IAuthService authService, INavigationService navService) 
            : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
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
    }
}
