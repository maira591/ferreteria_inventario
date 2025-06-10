using Ferreteria.Domain.ViewModel.Security;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.Website.Models
{
    public class UserInfoModel
    {
        public int UserId { get; set; }

        [Display(Name = "Nombres Y Apellidos")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Correo")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; } = true;

        public bool IsLocked { get; set; }

        public int FailedAttempts { get; set; }

        [Display(Name = "Roles")]
        public string Roles { get; set; } = string.Empty;
        public string RolesNames { get; set; } = string.Empty;

        public List<UserRoleVM> UserRoles { get; set; } = [];
    }
}