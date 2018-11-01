using System.Threading;
using System.Threading.Tasks;
using Inside.Data.Context;

namespace Inside.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _dbFactory;
        private InsideContext _dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public InsideContext DbContext => _dbContext ?? (_dbContext = _dbFactory.Init());

        public void Commit()
        {
            DbContext.Commit();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}