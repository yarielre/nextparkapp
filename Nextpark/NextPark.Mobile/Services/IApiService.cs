using System.Net.Http;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IApiService
    {
        string AuthToken { get; set; }

        Task<Response> CheckConnection();
        Task<Response> Delete<TVm>(string endpoint, int id);
        Task<Response> Get<TVm>(string endpoint, int id);
        Task<Response> Get<TVm>(string endpoint);
        HttpClient GetHttpClient();
        Task<Response> Post<TVm>(string endpoint, TVm tvm);
        Task<Response> Put<TVm>(string endpoint, int id, TVm tvm);
    }
}