using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class ParkingDataService : BaseDataService<ParkingModel>
    {
        public List<ParkingModel> Parkings;

        public ParkingDataService(IApiService apiService) : base(apiService, ApiSettings.ParkingsEndPoint)
        {
        }
    }
}
