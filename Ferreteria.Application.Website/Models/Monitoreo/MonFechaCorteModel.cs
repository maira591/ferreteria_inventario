using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class MonFechaCorteModel
    {
        public int Id { get; set; }
        [Display(Name = "Corte")]
        public int Corte { get; set; }
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha Fin")]
        public DateTime FechaFin { get; set; }
        [Display(Name = "Fecha Hora Inicio Monitoreo")]
        public DateTime FechaHoraIniMonitoreo { get; set; }
        [Display(Name = "Fecha Hora Agregados")]
        public DateTime FechaHoraAgregados { get; set; }
        [Display(Name = "Fecha Hora Plan Visita")]
        public DateTime FechaHoraPlanvisita { get; set; }
        [Display(Name = "Fecha Hora Con Información SIAF")]
        public DateTime FechaHoraConInfoSiaf { get; set; }
        [Display(Name = "Fecha Hora Con Información CORE")]
        public DateTime FechaHoraConInfoCore { get; set; }
    }
}
