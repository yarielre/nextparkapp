using NextPark.Mobile.Core.Settings;
using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Mobile.Services.Data
{
    public interface IParkingCategoriesDataService : IBaseDataService<ParkingCategoryModel>
    {
    }
    public class ParkingCategoriesDataService : BaseDataService<ParkingCategoryModel>
    {
        public ParkingCategoriesDataService(ApiService apiService) : base(apiService, ApiSettings.ParkingCategoriesEndPoint)
        {
        }
    }
}
