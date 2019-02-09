using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IProfileService
    {
        Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model);
        Task<EditProfileModel> UpdateUserData(EditProfileModel model);
    }
}