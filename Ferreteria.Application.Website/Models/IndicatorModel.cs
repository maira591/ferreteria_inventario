using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class IndicatorModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Grupo")]
        public Guid GroupIndicatorId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Sigla")]
        public string Sigla { get; set; }

        [Display(Name = "Activo")]
        public bool IsEnabled { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ValuesCatalogModel ValueCatalogGroupIndicator { get; set; }
        public bool IsNew { get; set; }

    }
}