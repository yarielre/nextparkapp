using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;

namespace NextPark.Mobile.Services
{
    public interface IGeolocatorService
    {
        Task<IEnumerable<Address>> GetAddressForPosition(Position position);
        Task<Position> GetLocation();
        Task<IEnumerable<Position>> GetPositionForAddress(string address);
        bool IsAvailable();
    }
}