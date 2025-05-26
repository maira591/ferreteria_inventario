using System;

namespace Core.Application.Website.Models
{
    public class GraphicIndicatorModel
    {
        public Guid Id { get; set; }
        public Guid GraphicId { get; set; }
        public Guid IndicatorId { get; set; }
        public Guid? IndicatorAggregateId { get; set; }
        public string Axis { get; set; }
        public string TypeField { get; set; }
        public string DisplayText { get; set; }
        public string Color { get; set; }
        public bool IsEnabled { get; set; }
        public int Order { get; set; }
        public string Sign { get; set; }


        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public GraphicModel Graphic { get; set; }
        public IndicatorModel Indicator { get; set; }
        public ValuesCatalogModel IndicatorAggregate { get; set; }
        public bool IsNew { get; set; }
    }
}