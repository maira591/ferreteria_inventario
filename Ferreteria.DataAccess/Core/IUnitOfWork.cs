using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Ferreteria.DataAccess.Core
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>()
        where T : class;

        void SaveChanges();
        Task SaveChangesAsync();

        void SetAutoDetectChanges(bool enabled);

        Task<int> ExecuteProcedure(string storeProcedure, params object[] parameters);

        Task<List<T>> ExecuteSelect<T>(string sqlText, params object[] parameters) where T : class;
        Task<DataTable> ExecuteQuery(string query, bool changeColumnsByteToString = true);
        Task<int> ExecuteProcedure(string storeProcedure);
    }
}
