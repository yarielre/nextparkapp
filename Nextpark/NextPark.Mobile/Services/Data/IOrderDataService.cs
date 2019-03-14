using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IOrderDataService
    {
        Task<DataServiceResponse<OrderModel>> CreateOrderAsync(OrderModel model);
        Task<OrderModel> DeleteOrdersAsync(int id);
        Task<OrderModel> EditOrderAsync(int id, OrderModel order);
        Task<List<OrderModel>> GetAllOrdersAsync();
        Task<OrderModel> GetOrderAsync(int orderId);
        Task<OrderModel> RenovateOrderAsync(int id, OrderModel order);
        Task<OrderModel> TerminateOrderAsync(int id);
    }
}