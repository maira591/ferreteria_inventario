using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Nombre")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "El Nombre del Rol no puede contener espacios.")]
        public string Name { get; set; }
        [Display(Name = "Habilitado")]
        public bool Enabled { get; set; }
        [Display(Name = "Permisos")]
        public List<PrivilegeModel> Privileges { get; set; }
        public string CodeAuthApplication { get; internal set; }
        public string NameAuthApplication { get; internal set; }
        public bool Assigned { get; set; }
        [Display(Name = "Prefijo")]
        public string Prefix { get; set; }
        public List<SelectListItem> LstPrefix { get; set; }
    }

    public class PrivilegeModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Privilegio")]
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Enabled { get; set; }
        public bool Allowed { get; set; }

    }

}