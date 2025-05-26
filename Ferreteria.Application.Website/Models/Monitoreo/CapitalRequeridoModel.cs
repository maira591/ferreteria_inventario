using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class CapitalRequeridoModel
    {
        public int Id { get; set; }
        [Display(Name = "IPC")]
        public string Ipc { get; set; }
        [Display(Name = "Año")]
        public int Anio { get; set; }
        [Display(Name = "Capital Financiera")]
        public string CapitalFinanciera { get; set; }
        [Display(Name = "Capital Solidaria")]
        public string CapitalSolidaria { get; set; }
    }
}
