using AutoMapper;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.ViewModel;

namespace Ferreteria.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModelComplete>().ReverseMap();
        }
    }
}
