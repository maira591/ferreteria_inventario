using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class MatcamelBetasModel
    {
        public int Id { get; set; }
        [Display(Name = "Año")]
        public int Anio { get; set; }
        [Display(Name = "Tipo Entidad")]
        public int TipoEntidad { get; set; }
        [Display(Name = "Id Beta")]
        public string IdBeta { get; set; }
        public string Valor { get; set; }
    }
}
