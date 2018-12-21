using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IOrderDataService : IBaseDataService<OrderModel>
    {
        Task<OrderModel> RenovateOrder(RenovateOrder model);
        Task<ParkingModel> TerminateOrder(int id);
    }
}