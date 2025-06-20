﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria.DataAccess.Model
{
    #region Permission

    #endregion

    #region RolePermission

    [Table("role_permissions")]
    public class RolePermission
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Key]
        [Column("permission_id")]
        public int PermissionId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; }
    }

    #endregion
}
