using System;

namespace Core.Application.Website.Models
{
    public class ReportColumnModel
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public Guid TableId { get; set; }
        public string ColumnName { get; set; }
        public ReportModel Report { get; set; }
        public ValuesCatalogModel Table { get; set; }
    }
}