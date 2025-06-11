using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Ferreteria.Application.Website.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Código Producto")]
        public string ProductCode { get; set; } = string.Empty;

        [Display(Name = "Nombre")]

        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Precio")]

        public string Price { get; set; } = string.Empty;
        public string PriceMoney
        {
            get
            {
                if (decimal.TryParse(Price, out decimal priceDecimal))
                {
                    return priceDecimal.ToString("C", new CultureInfo("es-CO"));
                }
                return "$0";
            }
        }
        [Display(Name = "Cantidad")]

        public string StockQuantity { get; set; } = string.Empty;
        [Display(Name = "Categoría")]

        public int CategoryId { get; set; }

        [Display(Name = "Proveedor")]
        public int SupplierId { get; set; }

        [Display(Name = "Unidad")]
        public string Unit { get; set; } = string.Empty;

        [Display(Name = "Marca Producto")]
        public string Brand { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; } = true;

        public CategoryModel Category { get; set; } = new();

        public SupplierModel Supplier { get; set; } = new();

    }
}