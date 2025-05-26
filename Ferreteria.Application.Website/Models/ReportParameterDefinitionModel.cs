using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class ReportParameterDefinitionModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string Name { get; set; }
        [Display(Name = "Llave")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string ReplaceKey { get; set; }
        [Display(Name = "Tipo Dato")]
        public string DataType { get; set; }
        [Display(Name = "Tipo Lista")]
        public int? ListType { get; set; }
        [Display(Name = "Sentencia SQL")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string SQLSentence { get; set; }

        [Display(Name = "Obligatorio")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public bool IsRequired { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public bool IsNew { get; set; }
        public List<ReportParameterModel> ReportParameterList { get; set; }
    }
}