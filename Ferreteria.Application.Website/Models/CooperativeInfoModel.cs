using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class CooperativeInfoModel
    {
        [Display(Name = "Razón Social")]
        public string CompanyName { get; set; }

        [Display(Name = "NIT")]
        public string TaxId { get; set; }

        [Display(Name = "Sigla")]
        public string Acronym { get; set; }

        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Display(Name = "Municipio")]
        public string Municipality { get; set; }

        [Display(Name = "Departamento")]
        public string Department { get; set; }

        [Display(Name = "Código Cooperativa")]
        public string CooperativeCode { get; set; }

        [Display(Name = "Sitio Web")]
        public string Website { get; set; }

        [Display(Name = "Email Corporativo")]
        public string CorporateEmail { get; set; }
    }

}