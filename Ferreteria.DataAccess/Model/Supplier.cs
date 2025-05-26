using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region Category

    #endregion

    #region Supplier

    [Table("suppliers")]
    public class Supplier
    {
        [Key]
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("contact")]
        public string Contact { get; set; } = string.Empty;

        [Column("address")]
        public string Address { get; set; } = string.Empty;

        public List<Product> Products { get; set; } = new();
    }

    #endregion
}
