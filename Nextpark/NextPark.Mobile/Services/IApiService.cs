using System.Net.Http;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IApiService
    {
        string AuthToken { get; set; }

        Task<ApiResponse<TVm>> Get<TVm>(string endpoint);
        Task<ApiResponse<TVm>> Get<TVm>(string endpoint, int id);
        
        Task<ApiResponse<TVm>> Post<TParam, TVm>(string endpoint, TParam tParam);
        Task<ApiResponse<TVm>> Post<TVm>(string endpoint, TVm tvm);

        Task<ApiResponse<TVm>> Put<TVm>(string endpoint, int id, TVm tvm);
        Task<ApiResponse<TVm>> Put<TVm>(string url, TVm tvm);
        Task<ApiResponse<TVm>> Put<TParam, TVm>(string url, TParam tParam);     //I think is better pass the id instead of the complete object

        Task<ApiResponse<TVm>> Delete<TVm>(string url);                         
        Task<ApiResponse<TVm>> Delete<TVm>(string endpoint, int id);

        ApiResponse CheckConnection();
        HttpClient GetHttpClient();
    }
}