using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inside.Xamarin.Services
{
    public class GeolocatorService
    {
        public double Lat { get; set; }
        public double Lon { get; set; }

        public GeolocatorService()
        {
            Lat = 0;
            Lon = 0;
        }
                

        public async Task GetLocation()
        {
            try
            {
                var locator = CrossGeolocator.Current;

                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled) return;

                locator.DesiredAccuracy = 50;
                var location = await locator.GetPositionAsync();
                Lat = location.Latitude;
                Lon = location.Longitude;

            }
            catch (Exception){}
        }
    }
}
