using System.Threading;
using System.Threading.Tasks;

namespace Inside.Web.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}