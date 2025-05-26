using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class MaximunDateModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Fecha Corte")]
        public DateTime CutoffDate { get; set; }

        [Display(Name = "Fecha Máxima Transmisión")]
        public DateTime MaxDate { get; set; }
        public Guid CooperativeTypeId { get; set; }
        public Guid LoadTypeId { get; set; }

        [Display(Name = "Tipo Cooperativa")]
        public string GuidCooperativeTypeId { get; set; }

        [Display(Name = "Tipo Carga")]
        public string GuidLoadTypeId { get; set; }

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public ValuesCatalogModel Type { get; set; }
        public LoadTypeModel FkLoadType { get; set; }
    }
}