using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
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
        public List<PrivilegeModel> Privileges { get; set; }
    }

    public class PrivilegeModel
    {
        public int Id { get; set; }
        [Display(Name = "Privilegio")]
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }

    }

}