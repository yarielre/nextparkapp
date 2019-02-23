using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.UIModels;
using NextPark.Mobile.ViewModels;
using NextPark.Models;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.Services
{
    public interface IProfileService
    {
        Position LastMapPosition { get; set; }
        ParkingItem LastEditingParking { get; set; }

        List<UIParkingModel> ParkingList { get; set; }
        List<UIParkingModel> UserParkingList { get; set; }
        List<OrderModel> UserOrderList { get; set; }

        Task<bool> ChangePassword(ChangePasswordModel model);
        Task<UserModel> EditProfile(EditProfileModel model);
        UIParkingModel GetParkingById(int searchId);
        OrderModel GetUserOrderById(int searchId);
        UIParkingModel GetUserParkingById(int searchId);
        Task<EditProfileModel> UpdateUserData(EditProfileModel model);
    }
}