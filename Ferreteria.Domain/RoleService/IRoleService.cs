using Ferreteria.Domain.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.RoleService
{
    public interface IRoleService
    {
        Task AddOrUpdateAsync(RoleVM role);
        Task<IEnumerable<RoleVM>> GetAllAsync();
        Task DeleteAsync(int id);
        Task<string> Validations(RoleVM role);
        Task<RoleVM> GetByIdAsync(int id);
    }
}
