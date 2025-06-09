using Ferreteria.DataAccess.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

    public class UserRoleVM
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public UserVM User { get; set; } = new();

        public RoleVM Role { get; set; } = new();
    }

    public class UserVM
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;

        public bool IsLocked { get; set; } = false;

        public int FailedAttempts { get; set; } = 0;

        public List<UserRoleVM> UserRoles { get; set; } = new();
    }
}
