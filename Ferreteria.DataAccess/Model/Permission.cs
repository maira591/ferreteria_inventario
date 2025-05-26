using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region UserRole

    #endregion

    #region Permission

    [Table("permissions")]
    public class Permission
    {
        [Key]
        [Column("permission_id")]
        public int PermissionId { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;

        public List<RolePermission> RolePermissions { get; set; } = new();
    }

    #endregion
}
