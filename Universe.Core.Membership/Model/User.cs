using Universe.Core.Data;
using Universe.Core.Mapping;

using Microsoft.AspNetCore.Identity;

namespace Universe.Core.Membership.Model;

public class User : IdentityUser<Guid>, IEntity<Guid>
{
	public User(string userName) : base(userName)
	{
	}


	public bool Approved { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public bool RootAdmin { get; set; }

	public bool TwoFactorRequired { get; set; }


	public IList<PasswordChange> PasswordChanges { get; set; } = new List<PasswordChange>();

	[DisableAutoIgnoreDefaultBehaviour]
	public IList<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

	[DisableAutoIgnoreDefaultBehaviour]
	public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
