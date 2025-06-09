using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region User

    #endregion

    #region Role

    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;

        public List<UserRole> UserRoles { get; set; } = [];

        public List<RolePermission> RolePermissions { get; set; } = [];
    }

    #endregion
}
