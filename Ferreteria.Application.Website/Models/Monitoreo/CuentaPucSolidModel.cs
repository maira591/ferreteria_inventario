using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class CuentaPucSolidModel
    {
        public bool Codmon { get; set; }
        [Display(Name = "Cuenta")]
        public string CuentaTxt { get; set; }
        public int Clase { get; set; }
        public int Grupo { get; set; }
        public int Cuenta { get; set; }
        public int SubCuenta { get; set; }
        [Display(Name = "Nombre Cuenta")]
        public string NombreCuenta { get; set; }
        [Display(Name = "Signo")]
        public string Signo { get; set; }
        [Display(Name = "Fecha Fin")]
        public DateTime? FechaFin { get; set; }
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        public int? IsNew { get; set; }

    }
}
