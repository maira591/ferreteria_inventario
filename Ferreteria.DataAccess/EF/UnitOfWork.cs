using Ferreteria.DataAccess.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Ferreteria.DataAccess.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories;

        private readonly FerreteriaContext _context;

        private bool _disposed;

        private const string oracleBeginEnd = "BEGIN {0} END;";

        public UnitOfWork(FerreteriaContext context)
        {
            this._context = context;
            this._repositories = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._context.Dispose();
            }
            this._disposed = true;
        }

        public IRepository<T> Repository<T>()
        where T : class
        {
            string name = typeof(T).Name;
            if (this._repositories.ContainsKey(name))
            {
                return (Repository<T>)this._repositories[name];
            }
            object obj = Activator.CreateInstance(typeof(Repository<>).MakeGenericType(new Type[] { typeof(T) }), new object[] { this._context });
            this._repositories.Add(name, obj);
            return (Repository<T>)this._repositories[name];
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw e;
            }
        }

        public void SetAutoDetectChanges(bool enabled)
        {
            //this._context.Configuration.AutoDetectChangesEnabled = enabled;
        }

        public async Task<int> ExecuteProcedure(string storeProcedure, params object[] parameters)
        {
            int result = await _context.Database.ExecuteSqlRawAsync(string.Format(oracleBeginEnd, storeProcedure), parameters);
            return result;
        }

        public async Task<int> ExecuteProcedure(string storeProcedure)
        {
            int result = await _context.Database.ExecuteSqlRawAsync(storeProcedure);
            return result;
        }

        public async Task<List<T>> ExecuteSelect<T>(string sqlText, params object[] parameters) where T : class
        {
            if (parameters == null)
            {
                return await _context.Set<T>().FromSqlRaw(sqlText).ToListAsync();
            }
            else
            {
                return await _context.Set<T>().FromSqlRaw(sqlText, parameters).ToListAsync();
            }
        }

        public async Task<System.Data.DataTable> ExecuteQuery(string query, bool changeColumnsByteToString = true)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;

                _context.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    var table = new System.Data.DataTable();
                    table.Load(result);

                    // returning DataTable (instead of DbDataReader), cause can't use DbDataReader after CloseConnection().
                    _context.Database.CloseConnection();

                    if (changeColumnsByteToString)
                    {
                        Utils.Util.ChangeColumnTypesId(table);
                    }
                    return table;
                }
            }
        }
    }
}
