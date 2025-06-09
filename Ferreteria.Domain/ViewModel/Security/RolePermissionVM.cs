namespace Ferreteria.Domain.ViewModel.Security
{
    public class RolePermissionVM
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public RoleVM Role { get; set; }
        public PermissionVM Permission { get; set; }
    }
}
