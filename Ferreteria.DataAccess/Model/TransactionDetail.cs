using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region Transaction

    #endregion

    #region TransactionDetail

    [Table("transaction_details")]
    public class TransactionDetail
    {
        [Key]
        [Column("transaction_detail_id")]
        public int TransactionDetailId { get; set; }

        [Column("transaction_id")]
        public int TransactionId { get; set; }

        public Transaction Transaction { get; set; } = new();

        [Column("product_id")]
        public int ProductId { get; set; }

        public Product Product { get; set; } = new();

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("subtotal")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Subtotal { get; private set; }
    }

    #endregion
}
