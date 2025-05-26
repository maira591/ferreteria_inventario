using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class MatInfoAdicionalModel
    {
        public int Id { get; set; }
        [Display(Name = "Tipo Entidad")]
        public int TipoEntidad { get; set; }
        [Display(Name = "Código Entidad")]
        public int CodigoEntidad { get; set; }
        [Display(Name = "Fecha Corte")]
        public DateTime FechaCorte { get; set; }
        [Display(Name = "Indicador")]
        public int IdIndicador { get; set; }
        [Display(Name = "Valor")]
        public string Valor { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NombreIndicador { get; set; }
        public string NombreTipoEntidad { get; set; }

    }
}
