using Ferreteria.DataAccess.Core;
using Ferreteria.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ferreteria.DataAccess.EF
{
    public class Repository<T> : IRepository<T>
where T : class
    {
        private readonly DbSet<T> _entitySet;

        private readonly FerreteriaContext _context;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Repository(FerreteriaContext context)
        {
            this._context = context;
            this._entitySet = context.Set<T>();
        }

        public void Add(T entity)
        {
            this._context.Entry<T>(entity).State = EntityState.Added;
        }

        public void AddOrUpdate(T entity)
        {
            var entry = _context.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    _context.Add(entity);
                    break;
                case EntityState.Modified:
                    _context.Update(entity);
                    break;
                case EntityState.Added:
                    _context.Add(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void BulkInsert(IEnumerable<T> entity)
        {
            _context.BulkInsert(entity);
        }

        public async Task BulkInsertAsync(IEnumerable<T> entity)
        {
            await _context.BulkInsertAsync(entity);
        }

        public async Task AddOrUpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    await _context.AddAsync(entity);
                    break;
                case EntityState.Modified:
                    await Task.Run(() => _context.Update(entity));
                    break;
                case EntityState.Added:
                    await _context.AddAsync(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //await this._entitySet.AddAsync(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            this._context.Set<T>().AddRange(entities);
        }

        public virtual long Count()
        {
            return (long)this._entitySet.AsNoTracking<T>().Count<T>();
        }

        public async Task<long> CountAsync()
        {
            return (long)(await (this._entitySet.AsNoTracking<T>().CountAsync<T>()));
        }


        public long Count(Expression<Func<T, bool>> where)
        {
            return (long)this._entitySet.AsNoTracking<T>().Where<T>(where).Count<T>();
        }


        public async Task<long> CountAsync(Expression<Func<T, bool>> where)
        {
            return (long)(await (this._entitySet.AsNoTracking<T>().Where<T>(where).CountAsync<T>()));
        }

        public void Detach(T entity)
        {
            ///this._context.Entry<T>(entity).State = EntityState.Detached;
        }

        public void DetachAll()
        {
            //IEnumerableExtensions.ForEach<DbEntityEntry<T>>(this._context.ChangeTracker.Entries<T>(), (DbEntityEntry<T> e) => e.State = EntityState.Detached);
        }

        public IQueryable<T> Entity()
        {
            return this._entitySet.AsNoTracking<T>().AsQueryable<T>();
        }

        public T Find(params object[] keyValues)
        {
            return this._entitySet.Find(keyValues);
        }

        public async Task<T> FindAsync(params object[] keyValues)
        {
            T entity = null;

            await Task.Run(() =>
            {
                entity = this._entitySet.Find(keyValues);
            });

            return entity;
        }

        public IEnumerable<T> Get()
        {
            return this._entitySet.AsNoTracking<T>();
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            IEnumerable<T> query = null;
            await Task.Run(() =>
            {
                query = this._entitySet.AsNoTracking<T>();
            });
            return query;
        }



        public IEnumerable<T> Get(Expression<Func<T, bool>> where)
        {
            return this._entitySet.Where<T>(where).AsNoTracking<T>();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> query = null;
            await Task.Run(() =>
            {
                query = this._entitySet.Where<T>(where).AsNoTracking<T>();
            });
            return query;
        }

        public IEnumerable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>();
            return ((IEnumerable<Expression<Func<T, object>>>)includes).Aggregate<Expression<Func<T, object>>, IQueryable<T>>(ts, (IQueryable<T> current, Expression<Func<T, object>> includeProperty) => EntityFrameworkQueryableExtensions.Include<T, object>(current, includeProperty));
        }

        public async Task<IEnumerable<T>> GetAsync(params Expression<Func<T, object>>[] includes)
        {
            IEnumerable<T> query = null;
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>();

            await Task.Run(() =>
            {
                query = ((IEnumerable<Expression<Func<T, object>>>)includes).Aggregate<Expression<Func<T, object>>, IQueryable<T>>(ts, (IQueryable<T> current, Expression<Func<T, object>> includeProperty) => EntityFrameworkQueryableExtensions.Include<T, object>(current, includeProperty));
            });
            return query;
        }

        public IEnumerable<T> Get(params string[] includes)
        {
            IEnumerable<T> query = null;
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>();


            var queryWithIncludes = ts;

            foreach (var include in includes)
                queryWithIncludes = queryWithIncludes.Include(include);

            query = queryWithIncludes;


            return query;
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> where, params string[] includes)
        {
            IEnumerable<T> query;
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>().Where(where);

            var queryWithIncludes = ts;

            foreach (var include in includes)
                queryWithIncludes = queryWithIncludes.Include(include);

            query = queryWithIncludes;

            return query;
        }

        public async Task<IEnumerable<T>> GetAsync(params string[] includes)
        {
            IEnumerable<T> query = null;
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>();

            await Task.Run(() =>
            {
                var queryWithIncludes = ts;

                foreach (var include in includes)
                    queryWithIncludes = queryWithIncludes.Include(include);

                query = queryWithIncludes;

            });
            return query;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where, params string[] includes)
        {
            IEnumerable<T> query = null;
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>().Where(where);

            await Task.Run(() =>
            {
                var queryWithIncludes = ts;

                foreach (var include in includes)
                    queryWithIncludes = queryWithIncludes.Include(include);

                query = queryWithIncludes;

            });
            return query;
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> ts = this._entitySet.Where<T>(where).AsNoTracking<T>();
            return ((IEnumerable<Expression<Func<T, object>>>)includes).Aggregate<Expression<Func<T, object>>, IQueryable<T>>(ts, (IQueryable<T> current, Expression<Func<T, object>> includeProperty) => EntityFrameworkQueryableExtensions.Include<T, object>(current, includeProperty));
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            IEnumerable<T> query = null;
            IQueryable<T> ts = this._entitySet.Where<T>(where).AsNoTracking<T>();

            await Task.Run(() =>
            {
                query = ((IEnumerable<Expression<Func<T, object>>>)includes).Aggregate<Expression<Func<T, object>>, IQueryable<T>>(ts, (IQueryable<T> current, Expression<Func<T, object>> includeProperty) => EntityFrameworkQueryableExtensions.Include<T, object>(current, includeProperty));
            });
            return query;
        }

        public IEnumerable<T> Get<TSort>(Expression<Func<T, bool>> where, Expression<Func<T, TSort>> order, int count = 0)
        {
            if (count != 0)
            {
                return this._entitySet.AsNoTracking<T>().Where<T>(where).OrderBy<T, TSort>(order).Take<T>(count);
            }
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>().Where<T>(where).OrderBy<T, TSort>(order);
            return ts;
        }

        public async Task<IEnumerable<T>> GetAsync<TSort>(Expression<Func<T, bool>> where, Expression<Func<T, TSort>> order, int count = 0)
        {
            IEnumerable<T> query = null;

            await Task.Run(() =>
            {
                if (count != 0)
                {
                    query = this._entitySet.AsNoTracking<T>().Where<T>(where).OrderBy<T, TSort>(order).Take<T>(count);
                }

                query = this._entitySet.AsNoTracking<T>().Where<T>(where).OrderBy<T, TSort>(order);
            });

            return query;
        }

        public IEnumerable<T> Get<TSort>(int pageNumber, int pageSize, Expression<Func<T, TSort>> order, SortOrder sortDirection = 0)
        {
            IEnumerable<T> ts;

            int num = (pageNumber - 1) * pageSize;
            if (sortDirection == SortOrder.Ascending)
            {
                ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Skip<T>(num).Take<T>(pageSize);
            }
            else
            {
                ts = (sortDirection == SortOrder.Descending ? this._entitySet.AsNoTracking<T>().OrderByDescending<T, TSort>(order).Skip<T>(num).Take<T>(pageSize) : this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Skip<T>(num).Take<T>(pageSize));
            }

            return ts;
        }

        public async Task<IEnumerable<T>> GetAsync<TSort>(int pageNumber, int pageSize, Expression<Func<T, TSort>> order, SortOrder sortDirection)
        {
            IEnumerable<T> ts = null;

            await Task.Run(() =>
            {
                int num = (pageNumber - 1) * pageSize;
                if (sortDirection == SortOrder.Ascending)
                {
                    ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Skip<T>(num).Take<T>(pageSize);
                }
                else
                {
                    ts = (sortDirection == SortOrder.Descending ? this._entitySet.AsNoTracking<T>().OrderByDescending<T, TSort>(order).Skip<T>(num).Take<T>(pageSize) : this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Skip<T>(num).Take<T>(pageSize));
                }
            });

            return ts;
        }

        public IEnumerable<T> Get<TSort>(int pageNumber, int pageSize, Expression<Func<T, TSort>> order, Expression<Func<T, bool>> where)
        {
            int num = (pageNumber - 1) * pageSize;
            return this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
        }
        public async Task<IEnumerable<T>> GetAsync<TSort>(int pageNumber, int pageSize, Expression<Func<T, TSort>> order, Expression<Func<T, bool>> where)
        {
            IEnumerable<T> query = null;

            await Task.Run(() =>
            {
                int num = (pageNumber - 1) * pageSize;
                query = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
            });

            return query;
        }

        public IEnumerable<T> Get<TSort>(int pageNumber, int pageSize, Expression<Func<T, TSort>> order, SortOrder sortDirection, Expression<Func<T, bool>> where)
        {
            IEnumerable<T> ts;
            int num = (pageNumber - 1) * pageSize;
            switch (sortDirection)
            {
                case SortOrder.Unspecified:
                    {
                        ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                        break;
                    }
                case SortOrder.Ascending:
                    {
                        ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                        break;
                    }
                case SortOrder.Descending:
                    {
                        ts = this._entitySet.AsNoTracking<T>().OrderByDescending<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                        break;
                    }
                default:
                    {
                        ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                        break;
                    }
            }
            return ts;
        }

        public async Task<IEnumerable<T>> GetAsync<TSort>(int pageNumber, int pageSize, Expression<Func<T, TSort>> order, SortOrder sortDirection, Expression<Func<T, bool>> where)
        {
            IEnumerable<T> ts = null;

            await Task.Run(() =>
            {
                int num = (pageNumber - 1) * pageSize;
                switch (sortDirection)
                {
                    case SortOrder.Unspecified:
                        {
                            ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                            break;
                        }
                    case SortOrder.Ascending:
                        {
                            ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                            break;
                        }
                    case SortOrder.Descending:
                        {
                            ts = this._entitySet.AsNoTracking<T>().OrderByDescending<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                            break;
                        }
                    default:
                        {
                            ts = this._entitySet.AsNoTracking<T>().OrderBy<T, TSort>(order).Where<T>(where).Skip<T>(num).Take<T>(pageSize);
                            break;
                        }
                }
            });

            return ts;
        }

        public void Remove(T entity)
        {
            //this._context.Entry<T>(entity).State = EntityState.Deleted;
        }

        public void Remove(Expression<Func<T, bool>> where)
        {
            foreach (T t in this._entitySet.Where<T>(where).AsEnumerable<T>())
            {
                this._entitySet.Remove(t);
            }
        }

        public virtual void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await this._context.SaveChangesAsync();
        }

        public IQueryable<T> Set()
        {
            return this._entitySet.AsNoTracking<T>().AsQueryable<T>();
        }

        public void Update(T entity)
        {
            this._context.Entry<T>(entity).State = EntityState.Modified;
        }

        public void Update(IEnumerable<T> entities)
        {
            Repository<T> repository = this;
            IEnumerableExtensions.ForEach<T>(entities, new Action<T>(repository.AddOrUpdate));
        }

        public async Task BulkUpdateAsync(IEnumerable<T> entity)
        {
            await _context.BulkUpdateAsync(entity);
        }
    }
}
