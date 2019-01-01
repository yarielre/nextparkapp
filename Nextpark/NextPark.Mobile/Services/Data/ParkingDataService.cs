using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class ParkingDataService : BaseDataService<ParkingModel>
    {
        public ParkingDataService(ApiService apiService) : base(apiService, ApiSettings.ParkingsEndPoint)
        {
        }
    }
}
