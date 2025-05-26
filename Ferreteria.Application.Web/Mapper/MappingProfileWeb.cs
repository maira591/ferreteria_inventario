using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Domain.ViewModel;

namespace Ferreteria.Application.Website.Mapping
{
    public class MappingProfileWeb : Profile
    {
        public MappingProfileWeb()
        {
            CreateMap<UserModel, UserBasicModel>().ReverseMap();
            CreateMap<RoleModel, Domain.ViewModel.Login.RoleModel>().ReverseMap();
            CreateMap<PrivilegeModel, Domain.ViewModel.Login.PrivilegeModel>().ReverseMap();
            CreateMap<UserModelComplete, UserBasicModel>().ReverseMap();
        }
    }
}
