using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region Product

    #endregion

    #region User

    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("email")]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Column("password")]
        [Required]
        public string Password { get; set; } = string.Empty;

        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;

        [Column("is_locked")]
        public bool IsLocked { get; set; } = false;

        [Column("failed_attempts")]
        public int FailedAttempts { get; set; } = 0;

        public List<UserRole> UserRoles { get; set; } = new();

        public List<Transaction> Transactions { get; set; } = new();
    }

    #endregion
}
