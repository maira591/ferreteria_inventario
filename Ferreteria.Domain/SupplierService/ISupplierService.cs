using Ferreteria.Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.SupplierService
{
    public interface ISupplierService
    {
        Task AddOrUpdateAsync(SupplierVM obj);
        Task<IEnumerable<SupplierVM>> GetAllAsync();
        Task DeleteAsync(int id);
        Task<string> Validations(SupplierVM obj);
        Task<SupplierVM> GetByIdAsync(int id);
    }
}
