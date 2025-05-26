using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class ValidationModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Descipción")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(4000)]
        public string Description { get; set; }
        [Display(Name = "Formato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(4000)]
        public string Format { get; set; }
        [MaxLength(4000)]
        public string ModelFormula { get; set; }
        [MaxLength(4000)]
        public string Formula { get; set; }
        [Display(Name = "Fecha de expiración")]
        public DateTime? ExpirationDate { get; set; }
        [Display(Name = "Tipo de validación")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Type { get; set; }
        [Display(Name = "Correctiva")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public bool Error { get; set; }
        public List<SelectListItem> ListTypes { get; set; }
        public List<SelectListItem> ListFormats { get; set; }
        [Display(Name = "Solo SQL")]
        public bool IsOnlySQL { get; set; }
    }
}