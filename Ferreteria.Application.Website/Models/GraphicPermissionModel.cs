using System;

namespace Core.Application.Website.Models
{
    public class GraphicPermissionModel
    {
        public Guid Id { get; set; }
        public Guid GraphicId { get; set; }
        public string RoleCode { get; set; }
        public GraphicModel Graphic { get; set; }
    }
}