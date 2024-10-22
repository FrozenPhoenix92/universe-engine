using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Audit;

public class AuditMappingProfile : Profile
{
    public AuditMappingProfile()
    {
        CreateMap<ChangeLog, ChangeLogDto>()
            .ForMember(d => d.State, opt => opt.MapFrom(src => Enum.GetName(typeof(EntityState), src.State)));
    }
}
