using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class FormatoModel
    {
        public int Codfor { get; set; }
        [Display(Name = "Nombre")]
        public string Nomfor { get; set; }
        [Display(Name = "Circular")]
        public int? Circular { get; set; }
        [Display(Name = "Fecha Circular")]
        public DateTime? FechaCircular { get; set; }
        [Display(Name = "Tabla Almacena")]
        public string TablaAlmacena { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Entidad Aplica")]
        public int EntidadAplica { get; set; }
    }
}
