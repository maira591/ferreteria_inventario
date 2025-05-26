using System;

namespace Core.Application.Website.Models
{
    public class FormatPeriodicityModel
    {
        public Guid Id { get; set; }
        public Guid PeriodicityId { get; set; }
        public virtual PeriodicityModel Periodicity { get; set; }
    }
}
