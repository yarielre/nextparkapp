using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IParkingDataService
    {
        Task<List<ParkingModel>> Get();
        Task<ParkingModel> Post(ParkingModel model);
        Task<ParkingModel> Put(ParkingModel model);
    }
}