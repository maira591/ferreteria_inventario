using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{

    #region Transaction

    [Table("transactions")]
    public class Transaction
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Column("user_id")]
        public int UserId { get; set; }

        public User User { get; set; } = new();

        [Column("inventory_id")]
        public int InventoryId { get; set; }

        public Inventory Inventory { get; set; } = new();

        public List<TransactionDetail> TransactionDetails { get; set; } = new();
    }

    #endregion
}
