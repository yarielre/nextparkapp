using System.Threading.Tasks;
using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.Services.Services
{
    public interface IOrderApiService
    {
        Task<ApiResponse> TerminateOrder(int orderId);
        Task UpdateOrderScheduler(Order updatedOrder);
    }
}