using Ferreteria.DataAccess.Model;
using System.Collections.Generic;

namespace Ferreteria.Domain.ViewModel.Security
{
    public class RoleVM
    {
        public int RoleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;

        public string Permissions { get; set; }

        public List<RolePermissionVM> RolePermissions { get; set; } = new();

        public List<UserRole> UserRoles { get; set; } = new();
    }
}
