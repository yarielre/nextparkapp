using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IParkingDataService 
    {
        ApiService ApiService { get; }

        Task<ParkingModel> Delete(int id);
        Task<List<ParkingModel>> Get();
        Task<ParkingModel> Get(int id);
        Task<ParkingModel> Post(ParkingModel model);
        Task<ParkingModel> Put(ParkingModel model, int id);
    }
    public class ParkingDataService : BaseDataService<ParkingModel>
    {
        public ParkingDataService(ApiService apiService) : base(apiService, ApiSettings.ParkingsEndPoint)
        {
        }
    }
}
