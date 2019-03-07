using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IPurchaseDataService
    {
        Task<PurchaseModel> BuyAmount(PurchaseModel model);
        Task<PurchaseModel> DrawalCash(PurchaseModel model);
    }
}