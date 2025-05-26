using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Recaudo
{
    public class EntidadesModel
    {
        [Display(Name = "Código Entidad")]
        public int CodigoEntidad { get; set; }
        public int TipoEntidad { get; set; }
        [Display(Name = "Nombre Organización")]
        public string Nombre { get; set; }
        [Display(Name = "NIT")]
        public string Nit { get; set; }
        [Display(Name = "Sigla")]
        public string Sigla { get; set; }
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }
        [Display(Name = "Municipio")]
        public int Municipio { get; set; }
        [Display(Name = "Departamento")]
        public int Departamento { get; set; }
        [Display(Name = "Teléfono 1")]
        public int Telefono1 { get; set; }
        [Phone]
        [Display(Name = "Teléfono 2")]
        public string Telefono2 { get; set; }
        [Display(Name = "Correo 1")]
        public string Correo1 { get; set; }
        [Display(Name = "Correo 2")]
        public string Correo2 { get; set; }
        [Display(Name = "Correo 3")]
        public string Correo3 { get; set; }
    }
}
