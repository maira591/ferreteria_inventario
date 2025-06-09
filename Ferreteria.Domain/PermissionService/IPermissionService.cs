using Ferreteria.Domain.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.PermissionService
{
    public interface IPermissionService
    {
        Task AddOrUpdateAsync(PermissionVM loadType);
        Task<IEnumerable<PermissionVM>> GetAllAsync();
        Task DeleteAsync(int id);
        Task<string> Validations(PermissionVM loadType);
        Task<PermissionVM> GetByIdAsync(int id);
    }
}
