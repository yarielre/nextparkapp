using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.DataInterface
{
    public interface IParkingDataService
    {
        Task<ParkingModel> CreateParkingAsync(ParkingModel model);                   //Create parking [Post]
        Task<ParkingModel> GetParkingAsync(int parkingId);                           //Get parking by parkingId [Get]
        Task<List<ParkingModel>> GetAllParkingsAsync();                              //Get all parkings [Get]
        Task<ParkingModel> EditParkingAsync(int id, ParkingModel parkingModel);     //Update parking [Put]
        Task<ParkingModel> DeleteParkingsAsync(int id);                              //Delete parking [Delete]                     
    }
    
}