namespace Ferreteria.Application.Website.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int CategoryId { get; set; }

        public CategoryModel Category { get; set; } = new();

        public int SupplierId { get; set; }

        public SupplierModel Supplier { get; set; } = new();

    }
}