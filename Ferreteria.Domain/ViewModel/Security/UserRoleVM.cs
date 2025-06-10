namespace Ferreteria.Domain.ViewModel.Security
{
    public class UserRoleVM
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public UserVM User { get; set; } = new();

        public RoleVM Role { get; set; } = new();
    }
}
