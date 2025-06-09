using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Domain.ViewModel;
using Ferreteria.Domain.ViewModel.Security;

namespace Ferreteria.Application.Website.Mapping
{
    public class MappingProfileWeb : Profile
    {
        public MappingProfileWeb()
        {
            CreateMap<UserModel, UserBasicModel>().ReverseMap();
            CreateMap<RoleModel, Domain.ViewModel.Login.RoleModel>().ReverseMap();
            CreateMap<PermissionModel, Domain.ViewModel.Login.PrivilegeModel>().ReverseMap();
            CreateMap<UserModelComplete, UserBasicModel>().ReverseMap();
            CreateMap<PermissionModel, PermissionVM>().ReverseMap();
            CreateMap<RoleModel, RoleVM>().ReverseMap();
        }
    }
}
