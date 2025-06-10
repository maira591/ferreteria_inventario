using System.Collections.Generic;

namespace Ferreteria.Domain.ViewModel
{
    public class SupplierVM
    {
        public int SupplierId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public List<ProductVM> Products { get; set; } = new();
    }
}
