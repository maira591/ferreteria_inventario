using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region Supplier

    #endregion

    #region Product

    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("unit")]
        public string Unit { get; set; } = string.Empty;

        [Column("brand")]
        public string Brand { get; set; } = string.Empty;
            
        [Column("is_enabled")]
        public bool IsEnabled { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("stock_quantity")]
        public int StockQuantity { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = new();

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; } = new();

        public List<TransactionDetail> TransactionDetails { get; set; } = new();
    }

    #endregion
}
