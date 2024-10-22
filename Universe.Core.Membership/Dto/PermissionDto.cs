using Universe.Core.Infrastructure;

namespace Universe.Core.Membership.Dto;

public class PermissionDto : IDto<Guid>
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}
