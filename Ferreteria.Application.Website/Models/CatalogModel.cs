using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class CatalogModel
    {
        public Guid Id { get; set; }
        public Guid AppId { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50)]

        public string Code { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Descripción")]
        [MaxLength(1000)]
        public string Description { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual List<ValuesCatalogModel> ValueCatalogs { get; set; }
    }
}