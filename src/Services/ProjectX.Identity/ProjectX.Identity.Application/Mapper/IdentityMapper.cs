using AutoMapper;
using ProjectX.Identity.Domain;
using System.Linq;

namespace ProjectX.Identity.Application.Mapper
{
    public class IdentityMapper : Profile
    {
        public IdentityMapper()
        {
            CreateMap<UserEntity, UserDto>()
                .ForMember(d => d.Roles, v => v.MapFrom(e => e.UserRoles.Select(r => r.Role)));

            CreateMap<Address, AddressDto>();
            CreateMap<RoleEntity, RoleDto>();
        }
    }
}
