using System;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Domain.ViewModel.Formulator
{
    public class ValidationVM
    {
        /// <summary>
        /// Id de la validacion
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Descripcion de la validacion
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(4000)]
        public string Description { get; set; }
        /// <summary>
        /// Codigo del valor catalogo del formato
        /// </summary>
        [Display(Name = "Formato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(4000)]
        public string Format { get; set; }
        /// <summary>
        /// Id del tipo de formato
        /// </summary>
        public Guid FormatTypeId { get; set; }
        /// <summary>
        /// Modelo que genera la formula
        /// </summary>
        [MaxLength(4000)]
        public string ModelFormula { get; set; }
        /// <summary>
        /// Sql que ejecuta la validacion
        /// </summary>
        [MaxLength(4000)]
        public string Formula { get; set; }
        /// <summary>
        /// tiempo hasta el qu se encuentra vigente la formula
        /// </summary>
        [Display(Name = "Fecha de expiración")]
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// Codigo del valor catalogo del tipo
        /// </summary>
        [Display(Name = "Tipo de validación")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Type { get; set; }
        /// <summary>
        /// Tipo de formato
        /// </summary>
        [Display(Name = "Tipo de formato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string TypeFormats { get; set; }
        /// <summary>
        /// Indica sies correctivo o no
        /// </summary>
        [Display(Name = "Correctiva")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public bool Error { get; set; }

        public Guid MessageId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Solo SQL")]
        public bool IsOnlySQL { get; set; }
    }
}
