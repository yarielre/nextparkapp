using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private CustomControls.CustomMap Map { get; set; }
        private readonly IGeolocatorService _geoLocatorService;

        public HomeViewModel(IGeolocatorService geolocatorService,
            INavigationService navService, 
            IApiService apiService, 
            IAuthService authService) : base(navService, apiService, authService)
        {
            _geoLocatorService = geolocatorService;
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
            var getLocationTask = _geoLocatorService.GetLocation();
            getLocationTask.Wait();
            var lastKnowPosition = getLocationTask.Result;

            var position = new Position(lastKnowPosition.Latitude, lastKnowPosition.Longitude);
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
        }
    }
}
