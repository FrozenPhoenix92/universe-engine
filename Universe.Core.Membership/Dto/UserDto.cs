using Universe.Core.Infrastructure;

namespace Universe.Core.Membership.Dto;

public class UserDto : IDto<Guid>
{
    public string? Email { get; set; }

	public string? FirstName { get; set; }

	public Guid Id { get; set; }

	public string? LastName { get; set; }

	public string? Password { get; set; }

	public IList<PasswordChangeDto> PasswordChanges { get; set; } = new List<PasswordChangeDto>();

	public string? PhoneNumber { get; set; }

	public bool RootAdmin { get; set; }

	public string? UserName { get; set; }

	public IList<UserPermissionDto> UserPermissions { get; set; } = new List<UserPermissionDto>();

	public IList<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
}
