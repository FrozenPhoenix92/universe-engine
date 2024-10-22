using Microsoft.AspNetCore.Authorization;

namespace Universe.Core.Membership.Authorization;

internal class PermissionRequirement : IAuthorizationRequirement
{
	public PermissionRequirement(string permission) => Permission = permission;

	public string Permission { get; private set; }
}
