namespace Universe.Core.Membership.Dto;

public class UserPermissionDto
{
    public Guid UserId { get; set; }

    public UserDto? User { get; set; }

    public Guid PermissionId { get; set; }

    public PermissionDto? Permission { get; set; }
}
