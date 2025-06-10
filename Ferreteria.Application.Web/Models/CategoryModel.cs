using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.Website.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;
    }
}