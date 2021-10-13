using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UrlShortener.Data.Repositories
{
    public interface IRepository<TId, TEntity>
    {
        public Task<TEntity> GetAsync(TId id);

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

        public Task<IEnumerable<TEntity>> GetAllAsync();

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, QueryOptions queryOptions);

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression);

        public Task InsertAsync(TEntity entity);
    }
}