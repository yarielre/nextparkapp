using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IParkingDataService : IBaseDataService<EventModel>
    {
    }
    public class ParkingDataService : BaseDataService<ParkingModel>
    {
        public ParkingDataService(ApiService apiService) : base(apiService, ApiSettings.ParkingsEndPoint)
        {
        }
    }
}
