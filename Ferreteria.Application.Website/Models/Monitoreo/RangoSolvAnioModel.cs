using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class RangoSolvAnioModel
    {
        public int Id { get; set; }
        [Display(Name = "Año")]
        public int Anio { get; set; }
        public string Ipc { get; set; }
        [Display(Name = "Patrimonio Técnico 1")]
        public string Pattec1 { get; set; }
        [Display(Name = "Patrimonio Técnico 2")]
        public string Pattec2 { get; set; }
        [Display(Name = "Patrimonio Técnico 3")]
        public string Pattec3 { get; set; }
        public string PreviousPattec1 { get; set; }
        public string PreviousPattec2 { get; set; }
        public string PreviousPattec3 { get; set; }
    }
}