using System.Net.Http;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IApiService
    {
        string AuthToken { get; set; }

        Task<ApiResponse> CheckConnection();
        Task<ApiResponse> Delete<TVm>(string url);
        Task<ApiResponse> Delete<TVm>(string endpoint, int id);
        Task<ApiResponse> Get<TVm>(string endpoint);
        Task<ApiResponse> Get<TVm>(string endpoint, int id);
        HttpClient GetHttpClient();
        Task<ApiResponse> Post<TParam, TVm>(string endpoint, TParam tvm);
        Task<ApiResponse> Post<TVm>(string endpoint, TVm tvm);
        Task<ApiResponse> Put<TVm>(string endpoint, int id, TVm tvm);
        Task<ApiResponse> Put<TVm>(string url, TVm tvm);
    }
}