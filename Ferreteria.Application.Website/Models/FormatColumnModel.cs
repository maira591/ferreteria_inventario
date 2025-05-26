using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class FormatColumnModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Formato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(4000)]
        public string GuidFormato { get; set; }
        [Display(Name = "Formato")]
        public string FormatName { get; set; }
        public Guid FormatId { get; set; }
        [Display(Name = "Homologación")]
        public Guid? HomologationId { get; set; }
        [Display(Name = "Nombre Columna")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(30)]
        public string NombreColumna { get; set; }
        [Display(Name = "Tipo de Dato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(20)]
        public string TipoDato { get; set; }
        [Display(Name = "Acepta Nulos")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public bool AceptaNulos { get; set; }
        [Display(Name = "Orden")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Orden { get; set; }
        [Display(Name = "Nombre a Mostrar")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(400)]
        public string NombreColumnaExcel { get; set; }
        [Display(Name = "Longitud")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string LongitudDato { get; set; }

        public FormatModel Format { get; set; }
        public HomologationModel Homologation { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsNew { get; set; }
        public string SelectAceptaNulos { get; set; }
        [Display(Name = "Columna Almacenamiento")]
        [StringLength(30)]
        public string StorageColumn { get; set; }

        [Display(Name = "Formulado")]
        public bool IsCalculated { get; set; }

        [Display(Name = "Formula")]
        [StringLength(4000)]
        public string Formula { get; set; }
    }
}