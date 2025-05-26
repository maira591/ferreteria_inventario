using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class ValuesCatalogModel
    {
        public Guid Id { get; set; }

        [Display(Name = "¿Cifrado?")]
        public bool IsEncrypted { get; set; }

        public Guid CatalogId { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Code { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }


        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string NameDisplay
        {
            get
            {
                return IsEncrypted ? "[Cifrado]" : Name;
            }
        }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "¿Cifrado?")]

        public string TextIsEncrypted { get { return IsEncrypted ? "Sí" : "No"; } }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid AppId { get; set; }
    }
}