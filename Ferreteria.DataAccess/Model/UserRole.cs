using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region Role

    #endregion

    #region UserRole

    [Table("user_roles")]
    public class UserRole
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        public User User { get; set; } = new();

        public Role Role { get; set; } = new();
    }

    #endregion
}
