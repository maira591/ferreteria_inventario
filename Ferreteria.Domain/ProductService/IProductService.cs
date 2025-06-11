using Ferreteria.Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferreteria.Domain.ProductService
{
    public interface IProductService
    {
        Task AddOrUpdateAsync(ProductVM obj);
        Task<IEnumerable<ProductVM>> GetAllAsync();
        Task<string> Validations(ProductVM obj);
        Task<ProductVM> GetByIdAsync(int id);
    }
}
