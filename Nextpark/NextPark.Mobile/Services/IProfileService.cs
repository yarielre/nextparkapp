using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.UIModels;
using NextPark.Models;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.Services
{
    public interface IProfileService
    {
        Position LastMapPosition { get; set; }

        List<UIParkingModel> ParkingList { get; set; }
        List<UIParkingModel> UserParkingList { get; set; }
        List<OrderModel> UserOrderList { get; set; }

        Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model);
        Task<EditProfileModel> UpdateUserData(EditProfileModel model);

        UIParkingModel GetParkingById(int searchId);
        UIParkingModel GetUserParkingById(int searchId);
        OrderModel GetUserOrderById(int searchId);
    }
}