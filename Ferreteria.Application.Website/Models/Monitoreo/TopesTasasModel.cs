using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class TopesTasasModel
    {
        public int Id { get; set; }
        [Display(Name = "Fecha Inicio")]
        public DateTime? FecIni { get; set; }
        [Display(Name = "Fecha Fin")]
        public DateTime? FecFin { get; set; }
        [Display(Name = "Tasa de Usura")]
        public string TasaUsura { get; set; }
        [Display(Name = "Clasificación")]
        public int Clasificacion { get; set; }
        public string NombreClasificacion { get; set; }
    }
}
