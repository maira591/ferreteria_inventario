using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class FormatModel
    {
        public Guid Id { get; set; }
        public Guid FormatTypeId { get; set; }
        [Display(Name = "Periodicidad")]
        public string Periodicities { get; set; }
        public string PeriodicitiesNames { get; set; }

        [Display(Name = "Tipo Formato")]
        public string GuidFormatTypeId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string Alias { get; set; }

        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Descripción")]
        [MaxLength(500)]
        public string Description { get; set; }
        [Display(Name = "Obligatorio")]
        public bool IsRequired { get; set; }
        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        public Guid AppId { get; set; }
        [Display(Name = "Uso de Renglón y Unidad Captura")]
        public int UseUnitRow { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual List<FormatPeriodicityModel> FormatPeriodicityList { get; set; }
        public virtual List<FormatColumnModel> Columns { get; set; }
        public virtual ValuesCatalogModel Type { get; set; }

        [Display(Name = "Formato Traspuesto")]
        public bool IsTrasposed { get; set; }
        public bool IsNew { get; set; }
        [Display(Name = "Tabla Almacenamiento")]
        [MaxLength(30)]
        public string StorageTable { get; set; }

    }

}