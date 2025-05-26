using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class ConstantesModel
    {
        public int Id { get; set; }
        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(100)]
        public string Codigo { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(500)]
        public string Nombre { get; set; }
        [Display(Name = "Padre")]
        public int? IdPadre { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Descripción")]
        [MaxLength(200)]
        public string Descripcion { get; set; }
        public string NombrePadre { get; set; }
    }
}
