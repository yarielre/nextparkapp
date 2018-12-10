using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Core.Services
{
    public interface IProfileService
    {
        Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model);
    }
}