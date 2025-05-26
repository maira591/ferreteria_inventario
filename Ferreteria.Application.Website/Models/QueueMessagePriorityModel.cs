using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class QueueMessagePriorityModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Tipo Cooperativa")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int EntityType { get; set; }
        [Display(Name = "Cooperativa")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int EntityCode { get; set; }
        [Display(Name = "Prioridad")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Priority { get; set; }
        [Display(Name = "Activo")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public bool IsEnabled { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityName { get; set; }

        public bool IsNew { get; set; }
    }
}