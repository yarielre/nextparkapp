using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Inside.Data.Context;
using Inside.Data.Extenssions;
using Inside.Data.Infrastructure;
using Inside.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace Inside.Data.Repositories
{
   public class BaseRepository<TEntity> :IRepository<TEntity> where TEntity:class ,IBaseEntity,new()
   {
       private InsideContext _dbContext;

       protected IDbFactory DbFactory
       {
           get;
           private set;
       }

       protected InsideContext DbContext => _dbContext ?? (_dbContext = DbFactory.Init());

       public BaseRepository(IDbFactory dbFactory)
       {
           DbFactory = dbFactory;
       }
        public void Add(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Added;
        }

        public void AddGraph(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }

       public void UpdateGraph(TEntity entity)
       {
            DbContext.Set<TEntity>().Add(entity);
            DbContext.ApplyChanges();
        }

       public void Delete(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Deleted;
        }

       public void Delete(IEnumerable<TEntity> entities)
       {
           foreach (var entity in entities)
           {
               DbContext.Entry(entity).State = EntityState.Deleted;
           }
       }

        public TEntity Find(int key)
        {
            return DbContext.Set<TEntity>().Find(key);
        }

       public TEntity Find(int? key)
       {
           return key != null ? DbContext.Set<TEntity>().Find(key) : null;
       }

       public async Task<TEntity> FindAsync(int key, CancellationToken cancellationToken = new CancellationToken())
        {
            return await DbContext.Set<TEntity>().FindAsync(cancellationToken, key);
        }

        public async Task<TEntity> SingleAsync(CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).SingleAsync(cancellationToken);
        }

        public async Task<TEntity> SingleWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).SingleAsync(criteria, cancellationToken);
        }

        public async Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity> SingleOrDefaultWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).SingleOrDefaultAsync(criteria, cancellationToken);
        }

        public async Task<TEntity> FirstAsync(CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).FirstAsync(cancellationToken);
        }

        public async Task<TEntity> FirstWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).FirstAsync(criteria, cancellationToken);
        }

        public async Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity> FirstOrDefaultWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).FirstOrDefaultAsync(criteria, cancellationToken);
        }

        public IQueryable<TEntity> All()
        {
            return DbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FindAllWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).Where(criteria).ToListAsync(cancellationToken);
        }

       public async Task<List<TEntity>> FindPagedAsync(int skip, int take, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).OrderBy(e => e.Id).Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FindPagedWhereAsync(Expression<Func<TEntity, bool>> criteria, int skip,
            int take, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await AllIncluding(includes).Where(criteria).OrderBy(e => e.Id).Skip(skip).Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<TCompare> MaxAsync<TCompare>(Expression<Func<TEntity, TCompare>> criteria,
            CancellationToken cancellationToken) where TCompare : IComparable
        {
            return await All().MaxAsync(criteria, cancellationToken);
        }

        public async Task<TCompare> MinAsync<TCompare>(Expression<Func<TEntity, TCompare>> criteria,
            CancellationToken cancellationToken) where TCompare : IComparable
        {
            return await All().MinAsync(criteria, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await All().CountAsync(cancellationToken);
        }

        public async Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken)
        {
            return await All().CountAsync(criteria, cancellationToken);
        }

        public async Task<long> CountLongAsync(CancellationToken cancellationToken)
        {
            return await All().LongCountAsync(cancellationToken);
        }

        public async Task<long> CountLongWhereAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken)
        {
            return await All().LongCountAsync(criteria, cancellationToken);
        }

        public async Task<List<TEntity>> OrderByAsync(Expression<Func<TEntity, object>> attribute,
            CancellationToken cancellationToken, bool desc = false, params Expression<Func<TEntity, object>>[] includes)
        {
            return desc
                ? await AllIncluding(includes).OrderByDescending(attribute).ToListAsync(cancellationToken)
                : await AllIncluding(includes).OrderBy(attribute).ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> OrderByPagedAsync(Expression<Func<TEntity, object>> attribute, int skip, int take,
             CancellationToken cancellationToken, bool desc = false, params Expression<Func<TEntity, object>>[] includes)
        {
            return desc
                ? await AllIncluding(includes).OrderByDescending(attribute).Skip(skip).Take(take).ToListAsync(cancellationToken)
                : await AllIncluding(includes).OrderBy(attribute).Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> OrderByWherePagedAsync(Expression<Func<TEntity, object>> attribute,
            Expression<Func<TEntity, bool>> criteria, int skip, int take,
            CancellationToken cancellationToken = new CancellationToken(), bool desc = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return desc
                ? await AllIncluding(includes).Where(criteria).OrderByDescending(attribute).Skip(skip).Take(take)
                    .ToListAsync(cancellationToken)
                : await AllIncluding(includes).Where(criteria).OrderBy(attribute).Skip(skip).Take(take)
                    .ToListAsync(cancellationToken);
        }
    }
}
