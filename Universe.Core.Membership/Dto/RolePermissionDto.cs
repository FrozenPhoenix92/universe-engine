namespace Universe.Core.Membership.Dto;

public class RolePermissionDto
{
    public Guid RoleId { get; set; }

    public RoleDto? Role { get; set; }

    public Guid PermissionId { get; set; }

    public PermissionDto? Permission { get; set; }
}
