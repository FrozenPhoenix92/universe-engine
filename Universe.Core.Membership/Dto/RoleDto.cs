using Universe.Core.Infrastructure;

namespace Universe.Core.Membership.Dto;

public class RoleDto : IDto<Guid>
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool TwoFactorRequired { get; set; }


    public IList<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();

    public IList<RolePermissionDto> RolePermissions { get; set; } = new List<RolePermissionDto>();
}
