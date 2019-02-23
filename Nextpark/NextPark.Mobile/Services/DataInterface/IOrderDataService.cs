using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services.DataInterface
{
    public interface IOrderDataService
    {
        Task<OrderModel> CreateOrderAsync(OrderModel model);                     //Create order [Post]
        Task<OrderModel> GetOrderAsync(int orderId);                             //Get order by orderId [Get]
        Task<List<OrderModel>> GetAllOrdersAsync();                              //Get all orders [Get]
        Task<OrderModel> EditOrderAsync(int id, OrderModel order);               //Update order [Put]
        Task<OrderModel> DeleteOrdersAsync(int id);                              //Delete order [Delete]          
    }
}