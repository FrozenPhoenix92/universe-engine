using AutoMapper;

using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;

namespace Universe.Core.Membership;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<UserRole, UserRoleDto>()
			.ReverseMap()
			.ForMember(d => d.User, m => m.Ignore())
			.ForMember(d => d.Role, m => m.Ignore());

		CreateMap<UserPermission, UserPermissionDto>()
			.ReverseMap()
			.ForMember(d => d.User, m => m.Ignore())
			.ForMember(d => d.Permission, m => m.Ignore());

		CreateMap<RolePermission, RolePermissionDto>()
			.ReverseMap()
			.ForMember(d => d.Permission, m => m.Ignore())
			.ForMember(d => d.Role, m => m.Ignore());
	}
}
