using NextPark.Mobile.Settings;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services
{
    public class GeolocatorService : IGeolocatorService
    {
        private readonly IGeolocator _geolocator;
        private readonly IPermissions _permissions;

        public GeolocatorService()
        {
            _geolocator = CrossGeolocator.Current;
            //TODO: Set other settings here
            _geolocator.DesiredAccuracy = GeoSettings.DesiredAccuracy;

            _permissions = CrossPermissions.Current;
        }

        public async Task<PermissionStatus> GetPermissionsStatus()
        {
            var status = await _permissions.CheckPermissionStatusAsync(Permission.Location);
            return status;
        }

        public async Task<bool> IsPermissionGaranted() {

            try
            {
                var status = await _permissions.CheckPermissionStatusAsync(Permission.Location);

                if (status != PermissionStatus.Granted)
                {
                    var showRequest = await _permissions.ShouldShowRequestPermissionRationaleAsync(Permission.Location);

                    if (showRequest)
                    {
                        var requestPermissions = await _permissions.RequestPermissionsAsync(Permission.Location);
                        //Best practice to always check that the key exists

                        if (requestPermissions.ContainsKey(Permission.Location))
                            status = requestPermissions[Permission.Location];
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {

                }
            }
            catch (Exception)
            {
                return false;

            }

            return false;
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
