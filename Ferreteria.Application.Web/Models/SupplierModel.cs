using Ferreteria.Domain.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.Website.Models
{
    public class SupplierModel
    {
        public int SupplierId { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Teléfono Contacto")]

        public string Contact { get; set; } = string.Empty;
        [Display(Name = "Dirección")]

        public string Address { get; set; } = string.Empty;

        public List<ProductModel> Products { get; set; } = new();
    }
}