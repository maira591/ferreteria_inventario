using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferreteria.Domain.ViewModel
{
    public class ProductVM
    {
        public int ProductId { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public bool IsEnabled { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int CategoryId { get; set; }

        public CategoryVM Category { get; set; } = new();

        public int SupplierId { get; set; }

        public SupplierVM Supplier { get; set; } = new();

    }
}
