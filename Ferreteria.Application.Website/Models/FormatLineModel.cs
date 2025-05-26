using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class FormatLineModel
    {
        public Guid Id { get; set; }
        public Guid FormatId { get; set; }
        public Guid CaptureUnitId { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Code { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(300)]
        public string Description { get; set; }

        [Display(Name = "Activo")]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public bool IsNew { get; set; }
        public CaptureUnitModel CaptureUnit { get; set; }

    }
}