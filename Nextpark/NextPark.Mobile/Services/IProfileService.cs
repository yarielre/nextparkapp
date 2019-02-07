using System.Threading.Tasks;
using NextPark.Models;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.Services
{
    public interface IProfileService
    {
        Position LastUserPosition { get; set; }

        Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model);
        Task<EditProfileModel> UpdateUserData(EditProfileModel model);
    }
}