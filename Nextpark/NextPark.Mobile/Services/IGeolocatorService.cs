using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;

namespace NextPark.Mobile.Services
{
    public interface IGeolocatorService
    {
        Task<PermissionStatus> GetPermissionsStatus();
        Task<IEnumerable<Address>> GetAddressForPosition(Position position);
        Task<Position> GetLocation();
        Task<IEnumerable<Position>> GetPositionForAddress(string address);
        bool IsAvailable();
        Task<bool> IsPermissionGaranted();
    }
}