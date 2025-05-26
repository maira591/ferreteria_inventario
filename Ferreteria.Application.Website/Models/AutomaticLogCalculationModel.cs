using System;

namespace Core.Application.Website.Models
{
    public class AutomaticLogCalculationModel
    {
        public Guid Id { get; set; }
        public DateTime CutoffDate { get; set; }
        public string EntityType { get; set; }
        public string EntityCode { get; set; }
        public string NameProcedure { get; set; }
        public bool ExecutionStatus { get; set; }
        public string Message { get; set; }
        public DateTime ExecutionDate { get; set; }
    }
}