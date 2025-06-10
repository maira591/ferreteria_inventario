using Ferreteria.Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.CategoryService
{
    public interface ICategoryService
    {
        Task AddOrUpdateAsync(CategoryVM obj);
        Task<IEnumerable<CategoryVM>> GetAllAsync();
        Task DeleteAsync(int id);
        Task<string> Validations(CategoryVM obj);
        Task<CategoryVM> GetByIdAsync(int id);
    }
}
