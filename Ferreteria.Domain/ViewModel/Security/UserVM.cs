using System.Collections.Generic;

namespace Ferreteria.Domain.ViewModel.Security
{
    public class UserVM
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;

        public bool IsLocked { get; set; }

        public int FailedAttempts { get; set; }

        public string Roles { get; set; }

        public List<UserRoleVM> UserRoles { get; set; } = [];
    }
}
