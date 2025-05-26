using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class HomologationValueModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Homologación")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public Guid HomologationId { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50)]
        public string Value { get; set; }

        [Display(Name = "Código Homologado")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50)]
        public string ValueApproved { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50)]
        public string Name { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public HomologationModel Homologation { get; set; }

        [Display(Name = "Homologación")]
        public string HomologationName { get; set; }
        public bool IsNew { get; set; }
    }
}