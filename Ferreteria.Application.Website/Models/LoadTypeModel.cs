using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class LoadTypeModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Tipo de Cooperativa")]
        public Guid CooperativeTypeId { get; set; }
        [Display(Name = "Tipo Formato")]
        public string GuidCooperativeTypeId { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string Name { get; set; }
        [Display(Name = "Periodicidad")]
        public Guid PeriodicityId { get; set; }
        public Guid AppId { get; set; }

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public ValuesCatalogModel Type { get; set; }
        public PeriodicityModel Periodicity { get; set; }


    }
}
