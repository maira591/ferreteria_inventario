using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Recaudo
{
    public class MunicipiosModel
    {
        public int Departamento { get; set; }
        public string NombreDepartamento { get; set; }
        [Display(Name = "Código Municipio Dane")]
        public string Municipio { get; set; }
        [Display(Name = "Nombre Municipio")]
        public string Descripcion { get; set; }
        [Display(Name = "Código Municipio")]
        public string CodMunicipio { get; set; }
        public int? IsNew { get; set; }
        public string PrevCodMunicipio { get; set; }
    }
}
