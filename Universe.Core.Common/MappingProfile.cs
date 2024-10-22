using System.Text.Json.Nodes;
using AutoMapper;

using Universe.Core.Common.Dto;
using Universe.Core.Common.Model;
using Universe.Core.JsonSerialization;

namespace Universe.Core.Common;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<AppSettingsSet, AppSettingsSetDto>()
			.ForMember(d => d.Value, m => m.MapFrom(p => JsonNode.Parse(p.Value ?? "{}", default, default)))
			.ReverseMap()
			.ForMember(d => d.Value, m => m.MapFrom(p => p.Value == null
				? null
				: p.Value.ToJsonString(JsonSerializerOptionsProvider.GetOptions(true))));
	}
}
