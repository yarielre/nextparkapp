using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public interface IOrderDataService
    {
        Task<ApiResponse<OrderModel>> CreateOrderAsync(OrderModel model);
        Task<OrderModel> DeleteOrdersAsync(int id);
        Task<ApiResponse<OrderModel>> EditOrderAsync(int id, OrderModel order);
        Task<List<OrderModel>> GetAllOrdersAsync();
        Task<OrderModel> GetOrderAsync(int orderId);
        Task<ApiResponse<OrderModel>> RenovateOrderAsync(int id, OrderModel order);
        Task<ApiResponse<OrderModel>> TerminateOrderAsync(int id);
    }
}