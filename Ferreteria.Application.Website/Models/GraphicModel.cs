using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class GraphicModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Grupo")]
        public Guid? GraphicGroupId { get; set; }
        [Display(Name = "Sub Grupo")]
        public Guid GraphicSubGroupId { get; set; }
        [Display(Name = "Nombre")]
        public string Title { get; set; }
        [Display(Name = "Sub Título")]
        public string SubTitle { get; set; }
        [Display(Name = "Título Eje Y2")]
        public string AxisTitleY1 { get; set; }
        [Display(Name = "Título Eje Y")]
        public string AxisTitleY { get; set; }
        [Display(Name = "Título Eje X")]
        public string AxisTitleX { get; set; }
        [Display(Name = "Tipo Gráfica")]
        public string TypeGraphic { get; set; }
        [Display(Name = "Posición Leyendas")]
        public string PositionLegends { get; set; }
        [Display(Name = "Sentencia SQL Gráfica")]
        public string SQLSentence { get; set; }
        [Display(Name = "Sentencia SQL Complementaria")]
        public string SQLSentenceSupplementary { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public GraphicGroupModel GraphicGroup { get; set; }
        public GraphicSubGroupModel GraphicSubGroup { get; set; }
        public bool IsNew { get; set; }
        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Permitir descargar información")]
        public bool IsDownloadable { get; set; }
        [Display(Name = "Mostrar elección de indicadores")]
        public bool ShowIndicator { get; set; }
        [Display(Name = "Utiliza cuenta de balances")]
        public bool UseAccount { get; set; }

        public List<GraphicPermissionModel> GraphicPermissionList { get; set; }

        [Display(Name = "Permisos")]
        public string GraphicPermissions { get; set; }
        public string IndicatorFields { get; set; }

        public string RoleNames { get; set; }

        [Display(Name = "Indicadores")]
        public string IndicatorsNames { get; set; }
        [Display(Name = "Orden")]
        public int? Order { get; set; }

        [Display(Name = "Tipo Cooperativa")]
        public string EntityTypeId { get; set; }

        [Display(Name = "Tipo Cooperativa")]
        public string EntityTypeName { get; set; }
        public string GraphicPermissionId { get; set; }

        public List<GraphicIndicatorModel> ListGraphicIndicator { get; set; }
    }


    public class RunGraphicModel
    {
        public string EntityCode { get; set; }
        public string EntityType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Guid GraphicGroupId { get; set; }
        [Display(Name = "Sub Grupo")]
        public Guid? GraphicSubGroupId { get; set; }
        public Guid PeriodicityId { get; set; }
        public string IndicatorsSiglas { get; set; }
    }
}