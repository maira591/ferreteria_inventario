using Ferreteria.Domain.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.UserRoleService
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleVM>> CreateAsync(UserRoleVM userRoleVM);
        Task DeleteByUserIdAsync(int userId);
        Task<IEnumerable<UserRoleVM>> GetByUserId(int roleId);
    }
}
