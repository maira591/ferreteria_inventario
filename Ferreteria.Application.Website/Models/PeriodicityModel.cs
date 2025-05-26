using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class PeriodicityModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Descripción")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Número de Días")]
        public int Days { get; set; }

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        public Guid AppId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public bool IsNew { get; set; }

    }
}