using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextPark.Services.Services
{
    public interface IOrderApiService
    {
        Task<ApiResponse> TerminateOrder(int orderId);
    }
}
