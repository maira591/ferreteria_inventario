using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    [Table("inventories")]
    public class Inventory
    {
        [Key]
        [Column("inventory_id")]
        public int InventoryId { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<Transaction> Transactions { get; set; } = new();
    }

}
