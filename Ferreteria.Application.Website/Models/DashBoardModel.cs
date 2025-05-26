using Core.Domain.GraphicModel;
using System.Collections.Generic;

namespace Core.Application.Website.Models
{
    public class DashBoardModel
    {
        public double Depositos { get; set; }
        public double Ahorradores { get; set; }
        public double SeguroDeposito { get; set; }
        public double CoberturaSeguroDeposito { get; set; }
        public double IRC { get; set; }
        public double IndiceCalidadCartera { get; set; }
        public double? IndiceMorosidadCartera { get; set; }
        public double CoberturaCartera { get; set; }

        public List<ResponseCharJs> Graphics { get; set; }

    }
}