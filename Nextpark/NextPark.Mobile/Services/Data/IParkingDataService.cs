using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IParkingDataService
    {
        Task<ParkingModel> CreateParkingAsync(ParkingModel model);
        Task<ParkingModel> DeleteParkingsAsync(int id);
        Task<List<EventModel>> DeleteParkingsEventsAsync(int id);
        Task<ParkingModel> EditParkingAsync(ParkingModel parkingModel);
        Task<List<ParkingModel>> GetAllParkingsAsync();
        Task<ParkingModel> GetParkingAsync(int parkingId);
        Task<List<EventModel>> GetParkingEventsAsync(int parkingId);
    }
}