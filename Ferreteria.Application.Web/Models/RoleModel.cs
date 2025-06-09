using Ferreteria.Domain.ViewModel.Security;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.Website.Models
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Nombre")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "El Nombre del Rol no puede contener espacios.")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Habilitado")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Permisos")]
        public List<PermissionModel> Privileges { get; set; } = new();
        [Display(Name = "Permisos")]
        public string Permissions { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public string PermissionsNames { get; set; } = string.Empty;

        public List<RolePermissionVM> RolePermissions { get; set; } = [];

    }

    public class PermissionModel
    {
        public int PermissionId { get; set; }
        [Display(Name = "Nombre Privilegio")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Código Privilegio")]
        public string Code { get; set; } = string.Empty;
        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
    }
}