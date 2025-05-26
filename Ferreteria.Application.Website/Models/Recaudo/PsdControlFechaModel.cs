using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Recaudo
{
    public class PsdControlFechaModel
    {
        [Display(Name = "Fecha Corte")]
        public DateTime FechaCorte { get; set; }
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha Final")]
        public DateTime FechaFinal { get; set; }
        [Display(Name = "Fecha Máxima Pago")]
        public DateTime FechaMaxPago { get; set; }
        [Display(Name = "Número Meses")]
        public int? NumeroMeses { get; set; }
        [Display(Name = "Número Días")]
        public int? NumeroDias { get; set; }
        public int? IsNew { get; set; }
    }

}
