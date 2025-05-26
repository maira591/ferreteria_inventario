using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class CaptureUnitModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Code { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public bool IsNew { get; set; }

    }
    public class ReportPermissionModel
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public string RoleCode { get; set; }
        public ReportModel Report { get; set; }
    }
}