using Core.Domain.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class FomatTypeConfigurationModel
    {
        public Guid Id { get; set; }
        public Guid FormatId { get; set; }
        public Guid FormatExtensionTypeId { get; set; }
        [Display(Name = "Número de la fila")]
        public int RowNumber { get; set; }
        [Display(Name = "Columna Excel")]
        public string ExcelColumn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsNew { get; set; }
        public ValueCatalogVM FormatExtensionType { get; set; }
        public FormatVM Format { get; set; }

    }
}