using Ferreteria.Domain.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.RolePermissionService
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionVM>> CreateAsync(RolePermissionVM RolePermissionVM);
        Task DeleteByRoleAsync(int roleId);
        Task<IEnumerable<RolePermissionVM>> GetByPermission(int permissionId);
    }
}
