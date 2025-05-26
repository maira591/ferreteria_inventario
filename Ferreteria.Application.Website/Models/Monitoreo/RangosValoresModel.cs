using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class RangosValoresModel
    {
        public int Id { get; set; }
        [Display(Name = "Tipo Rango")]
        public string TipoRango { get; set; }
        [Display(Name = "Rango Desde")]
        public long RangoDesde { get; set; }
        [Display(Name = "Rango Hasta")]
        public long RangoHasta { get; set; }
        public string NombreTipoRango { get; set; }
    }
}
