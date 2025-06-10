using System.Collections.Generic;

namespace Ferreteria.Domain.ViewModel
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<ProductVM> Products { get; set; } = new();
    }
}
