using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services.Data
{
    public interface IBaseDataService<TModel> where TModel : class, new()
    {
        ApiService ApiService { get; }

        Task<TModel> Delete(int id);
        Task<List<TModel>> Get();
        Task<TModel> Get(int id);
        Task<TModel> Post(TModel model);
        Task<TModel> Put(TModel model, int id);
    }
}