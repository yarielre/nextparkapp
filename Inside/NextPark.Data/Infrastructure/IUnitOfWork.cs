using System.Threading;
using System.Threading.Tasks;

namespace NextPark.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}