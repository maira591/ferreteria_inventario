using Ferreteria.Domain.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.UserService
{
    public interface IUserService
    {
        Task AddOrUpdateAsync(UserVM role);
        Task<IEnumerable<UserVM>> GetAllAsync();
        Task DeleteAsync(int id);
        Task<string> Validations(UserVM role);
        Task<UserVM> GetByIdAsync(int id);
    }
}
