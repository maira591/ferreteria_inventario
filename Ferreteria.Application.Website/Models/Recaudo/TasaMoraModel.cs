using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Application.Website.Models.Recaudo
{
    public class TasaMoraModel
    {
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha Final")]
        public DateTime FechaFinal { get; set; }
        [Display(Name = "Valor Tasa Mora")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public string ValorTasaMora { get; set; }
        public int? IsNew { get; set; }
    }

}
