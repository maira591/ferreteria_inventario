using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Recaudo
{
    public class DepartamentosModel
    {
        [Display(Name = "Código Departamento")]
        public string Departamento { get; set; }
        [Display(Name = "Nombre Departamento")]
        public string Descripcion { get; set; }
        public string Indicativo { get; set; }
        public int? IsNew { get; set; }
    }
}
