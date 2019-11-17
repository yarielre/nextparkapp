using System;
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
        MapType LastMapType { get; set; }
        ParkingItem LastEditingParking { get; set; }
        DateTime LastEditingEventDate { get; set; }
        bool UserReserveMode { get; set; }
        DateTime UserStartDate { get; set; }
        DateTime UserEndDate { get; set; }
        bool Updating { get; set; }

        List<UIParkingModel> ParkingList { get; set; }
        List<UIParkingModel> UserParkingList { get; set; }
        List<OrderModel> UserOrderList { get; set; }

        Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model);
        Task<EditProfileModel> UpdateUserData(EditProfileModel model);
        Task<bool> ChangePassword(ChangePasswordModel model);
        Task<bool> RefreshUserData();

        UIParkingModel GetParkingById(int searchId);
        UIParkingModel GetUserParkingById(int searchId);
        OrderModel GetUserOrderById(int searchId);
    }
}
