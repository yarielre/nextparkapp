using System.Threading;
using System.Threading.Tasks;

namespace NextPark.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _dbFactory;
        private ApplicationDbContext _dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public ApplicationDbContext DbContext => _dbContext ?? (_dbContext = _dbFactory.Init());

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