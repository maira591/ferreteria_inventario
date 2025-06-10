using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.Website.Models
{
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