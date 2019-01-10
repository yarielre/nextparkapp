using NextPark.Mobile.Core.Settings;
using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Mobile.Services.Data
{
    public class ParkingTypeDataService : BaseDataService<ParkingTypeModel>
    {
        public ParkingTypeDataService(ApiService apiService) : base(apiService, ApiSettings.ParkingTypesEndPoint)
        {
        }
    }
}
