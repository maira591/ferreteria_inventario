using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class ReportModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Vista Previa")]
        public bool PreView { get; set; }
        public string SQLSentence { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public bool IsNew { get; set; }
        [Display(Name = "Permisos")]
        public string ReportPermissions { get; set; }
        public string ReportPermissionId { get; set; }
        [Display(Name = "Parámetros")]
        public string ReportParameters { get; set; }
        public string ReportParameterId { get; set; }
        public string RoleNames { get; set; }
        public List<ReportPermissionModel> ReportPermissionList { get; set; }
        public List<ReportParameterModel> ReportParameterList { get; set; }

        [Display(Name = "Tabla")]
        public string CodeTable { get; set; }
        [Display(Name = "Solo SQL")]
        public bool IsOnlySQL { get; set; }

        [Display(Name = "Columnas")]
        public string ReportColumns { get; set; }
        public string ColumnReportId { get; set; }
        public List<ReportColumnModel> ReportColumnList { get; set; }

    }
}