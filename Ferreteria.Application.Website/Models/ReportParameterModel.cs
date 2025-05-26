using System;

namespace Core.Application.Website.Models
{
    public class ReportParameterModel
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public Guid ParameterDefinitionId { get; set; }
        public int? Order { get; set; }
        public ReportParameterDefinitionModel ReportParameterDefinition { get; set; }
    }
}