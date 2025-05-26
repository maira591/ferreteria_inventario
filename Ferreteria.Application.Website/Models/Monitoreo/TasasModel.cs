using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Monitoreo
{
    public class TasasModel
    {
        public int Id { get; set; }
        [Display(Name = "Nombre Tasa")]
        public string NombreTasa { get; set; }
        [Display(Name = "Fecha Tasa")]
        public DateTime FechaTasa { get; set; }
        [Display(Name = "Valor Tasa")]
        public string ValorTasa { get; set; }
    }
}
