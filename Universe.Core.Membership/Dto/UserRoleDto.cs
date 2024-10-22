namespace Universe.Core.Membership.Dto;

public class UserRoleDto
{
    public Guid UserId { get; set; }

    public UserDto? User { get; set; }

    public Guid RoleId { get; set; }

    public RoleDto? Role { get; set; }
}
