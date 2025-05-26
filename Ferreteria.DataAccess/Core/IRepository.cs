using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ferreteria.DataAccess.Core
{
    public interface IRepository<T>
        where T : class
    {
        void Add(T entity);

        void AddOrUpdate(T entity);

        Task AddOrUpdateAsync(T entity);

        void AddRange(IEnumerable<T> entities);

        long Count();
        Task<long> CountAsync();

        long Count(Expression<Func<T, bool>> where);
        Task<long> CountAsync(Expression<Func<T, bool>> where);

        void Detach(T entity);

        void DetachAll();

        IQueryable<T> Entity();

        T Find(params object[] keyValues);
        Task<T> FindAsync(params object[] keyValues);

        IEnumerable<T> Get();
        Task<IEnumerable<T>> GetAsync();

        IEnumerable<T> Get(params string[] includes);
        IEnumerable<T> Get(Expression<Func<T, bool>> where, params string[] includes);

        Task<IEnumerable<T>> GetAsync(params string[] includes);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where, params string[] includes);

        IEnumerable<T> Get(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where);

        IEnumerable<T> Get(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAsync(params Expression<Func<T, object>>[] includes);

        IEnumerable<T> Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);

        IEnumerable<T> Get<TOrder>(Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order, int count = 0);
        Task<IEnumerable<T>> GetAsync<TOrder>(Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order, int count = 0);

        IEnumerable<T> Get<TOrder>(int pageNumber, int pageSize, Expression<Func<T, TOrder>> order, SortOrder sortDirection);
        Task<IEnumerable<T>> GetAsync<TOrder>(int pageNumber, int pageSize, Expression<Func<T, TOrder>> order, SortOrder sortDirection);

        IEnumerable<T> Get<TOrder>(int pageNumber, int pageSize, Expression<Func<T, TOrder>> order, Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetAsync<TOrder>(int pageNumber, int pageSize, Expression<Func<T, TOrder>> order, Expression<Func<T, bool>> where);

        IEnumerable<T> Get<TOrder>(int pageNumber, int pageSize, Expression<Func<T, TOrder>> order, SortOrder sortDirection, Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetAsync<TOrder>(int pageNumber, int pageSize, Expression<Func<T, TOrder>> order, SortOrder sortDirection, Expression<Func<T, bool>> where);
        void BulkInsert(IEnumerable<T> entity);
        Task BulkInsertAsync(IEnumerable<T> entity);
        void Remove(T entity);

        void Remove(Expression<Func<T, bool>> where);

        void SaveChanges();
        Task SaveChangesAsync();

        IQueryable<T> Set();

        void Update(T entity);

        void Update(IEnumerable<T> entities);

        Task BulkUpdateAsync(IEnumerable<T> entity);
    }
}
