using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class GraphicSubGroupModel
    {
        public Guid Id { get; set; }
        public Guid GraphicGroupId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(500)]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(1000)]
        public string Description { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsNew { get; set; }

        public GraphicGroupModel GraphicGroup { get; set; }

        [Display(Name = "Grupo")]
        public string GraphicGroupName { get; set; }

        [Display(Name = "Nombre Grupo Indicador")]
        public string GroupIndicatorName { get; set; }
    }
}