using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Inside.Domain.Core;

namespace Inside.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, IBaseEntity, new()
    {
        void Add(TEntity entity);
        void AddGraph(TEntity entity);
        void Update(TEntity entity);
        void UpdateGraph(TEntity entity);
        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entity);
        TEntity Find(int key);
        TEntity Find(int? key);
        Task<TEntity> FindAsync(int key, CancellationToken cancellationToken = default(CancellationToken));

        Task<TEntity> SingleAsync(CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> SingleWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> SingleOrDefaultWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FirstAsync(CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FirstWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FirstOrDefaultWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> All();
        IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> FindAllWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> FindPagedAsync(int skip, int take,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> FindPagedWhereAsync(Expression<Func<TEntity, bool>> criteria, int skip, int take,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TCompare> MaxAsync<TCompare>(Expression<Func<TEntity, TCompare>> criteria,
            CancellationToken cancellationToken = default(CancellationToken)) where TCompare : IComparable;

        Task<TCompare> MinAsync<TCompare>(Expression<Func<TEntity, TCompare>> criteria,
            CancellationToken cancellationToken = default(CancellationToken)) where TCompare : IComparable;

        Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<long> CountLongAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<long> CountLongWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TEntity>> OrderByAsync(Expression<Func<TEntity, object>> attribute,
            CancellationToken cancellationToken = default(CancellationToken), bool desc = false,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> OrderByPagedAsync(Expression<Func<TEntity, object>> attribute, int skip, int take,
            CancellationToken cancellationToken = default(CancellationToken), bool desc = false,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> OrderByWherePagedAsync(Expression<Func<TEntity, object>> attribute,
            Expression<Func<TEntity, bool>> criteria, int skip, int take,
            CancellationToken cancellationToken = default(CancellationToken), bool desc = false,
            params Expression<Func<TEntity, object>>[] includes);
    }
}