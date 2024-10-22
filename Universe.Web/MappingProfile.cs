using AutoMapper;

using Universe.Web.Dto;
using Universe.Web.Model;

namespace Universe.Core.Common;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Client, ClientDto>()
			.ForMember(d => d.AccountConfirmed, m => m.MapFrom(p => !string.IsNullOrEmpty(p.PasswordHash)));
	}
}
