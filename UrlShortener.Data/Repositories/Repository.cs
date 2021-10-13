using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UrlShortener.Data.Factories;
using UrlShortener.Data.Models;

namespace UrlShortener.Data.Repositories
{
    public class Repository<TId, TEntity> : IRepository<TId, TEntity>
        where TEntity : Entity<TId>
    {
        /// <summary>
        /// For the purposes of this example, the "database" will be an in-memory collection
        /// </summary>
        private readonly static BlockingCollection<TEntity> _inMemoryLookup = new();

        private readonly IConnectionFactory _connectionFactory;

        /// <summary>
        /// This class represents a data access layer.
        /// Ordinarilly, something like Entity Framework or Dapper would be used.
        /// As stated above, a (thread-safe) in-memory collection will be used.
        /// Expressions are used in all parameters over functions so we can break them down in a realistic repository.
        /// Asyncronous operations are also used, as a database opertions usually are. For this example, operations will be syncronous.
        /// </summary>
        public Repository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public Task<TEntity> GetAsync(TId id)
            => GetAsync(t => id != null && id.Equals(t.Id));

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
            => QueryAsync(_ => Task.FromResult(_inMemoryLookup.Single(expression.Compile())));

        // Copy this to a list, so the raw collection can't ever be modified
        // Side-effect of an in-memory collection here.
        public Task<IEnumerable<TEntity>> GetAllAsync() => Task.FromResult(_inMemoryLookup.ToList().AsEnumerable());

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, QueryOptions queryOptions)
        {
            var filtered = (queryOptions.Order switch
            {
                Order.Ascending => _inMemoryLookup,
                Order.Descending => _inMemoryLookup.Reverse(),
                _ => throw new InvalidOperationException()
            }).Where(expression.Compile());

            if (queryOptions.Limit.HasValue)
            {
                filtered = filtered.Take(queryOptions.Limit.Value);
            }

            return Task.FromResult(filtered);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
            => Task.FromResult(_inMemoryLookup.Any(expression.Compile()));

        public Task InsertAsync(TEntity entity)
            => ExecuteAsync(_ =>
            {
                _inMemoryLookup.Add(entity);

                return Task.CompletedTask;
            });

        /// <summary>
        /// For the purposes of this challenge, this method won't be used properly.
        /// However, it defers the connection usage to a private method for disposable and re-usability ease
        /// </summary>
        /// <example>
        /// ExecuteAsync(con => con.ExecuteAsync("..."))
        /// </example>
        private async Task ExecuteAsync(Func<IDbConnection, Task> func)
        {
            using var connection = _connectionFactory.GetConnection();

            try
            {
                await func(connection);
            }
            catch (Exception ex) when (ex is not DataException)
            {
                throw new DataException("Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// As above.
        /// </summary>
        /// <example>
        /// QueryAsync(con => con.Get<T>("..."));
        /// </example>
        private async Task<T> QueryAsync<T>(Func<IDbConnection, Task<T>> func)
        {
            using var connection = _connectionFactory.GetConnection();

            try
            {
                return await func(connection);
            }
            catch (Exception ex) when (ex is not DataException)
            {
                throw new DataException("Unable to query database.", ex);
            }
        }
    }
}