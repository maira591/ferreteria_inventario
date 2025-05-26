using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class HomologationModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(60)]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(300)]
        public string Description { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public List<HomologationValueModel> Values { get; set; }
        public bool IsNew { get; set; }
    }
}