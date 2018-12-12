using NextPark.Mobile.Settings;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services
{
    public class GeolocatorService : IGeolocatorService
    {
        private readonly IGeolocator _geolocator;

        public GeolocatorService()
        {
            _geolocator = CrossGeolocator.Current;

            //TODO: Set other settings here
            _geolocator.DesiredAccuracy = GeoSettings.DesiredAccuracy;

        }

        public bool IsAvailable()
        {
            return !_geolocator.IsGeolocationAvailable || !_geolocator.IsGeolocationEnabled;
        }

        public async Task<Position> GetLocation()
        {
            try
            {
                return await _geolocator.GetLastKnownLocationAsync();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Geolocation service error: {0}", e.Message));
            }
        }

        public async Task<IEnumerable<Address>> GetAddressForPosition(Position position)
        {
            try
            {
                var address = await _geolocator.GetAddressesForPositionAsync(position);
                return address;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Geolocation service error: {0}", e.Message));

            }

        }
        public async Task<IEnumerable<Position>> GetPositionForAddress(string address)
        {
            try
            {
                var position = await _geolocator.GetPositionsForAddressAsync(address);
                return position;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Geolocation service error: {0}", e.Message));

            }

        }
    }
}
