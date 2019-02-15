using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class ParkingDataService : BaseDataService<ParkingModel>
    {
        public ParkingDataService(IApiService apiService) : base(apiService, ApiSettings.ParkingsEndPoint)
        {
        }
    }
}
