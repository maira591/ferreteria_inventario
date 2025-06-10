using AutoMapper;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.ViewModel;
using Ferreteria.Domain.ViewModel.Security;

namespace Ferreteria.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModelComplete>().ReverseMap();
            CreateMap<Permission, PermissionVM>().ReverseMap();
            CreateMap<Role, RoleVM>().ReverseMap();
            CreateMap<RolePermission, RolePermissionVM>().ReverseMap();
            CreateMap<UserRole, UserRoleVM>().ReverseMap();
            CreateMap<User, UserVM>().ReverseMap();
            CreateMap<Supplier, SupplierVM>().ReverseMap();
            CreateMap<Product, ProductVM>().ReverseMap();
            CreateMap<Category, CategoryVM>().ReverseMap();
        }
    }
}
