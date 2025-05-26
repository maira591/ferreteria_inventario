using System;
using System.Collections.Generic;

namespace Core.Application.Website.Models
{
    public class RunReportModel
    {
        public Guid ReportId { get; set; }
        public List<ReportParameters> Parameters { get; set; }
    }

    public class ReportParameters
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
