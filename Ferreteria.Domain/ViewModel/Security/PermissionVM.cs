

namespace Ferreteria.Domain.ViewModel.Security
{
    public class PermissionVM
    {
        public int PermissionId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;
    }
}
