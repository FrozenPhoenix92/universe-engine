using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Universe.Core.Membership.Authorization;

/// <summary>
/// Обработчик авторизации, проверяющий наличие прав.
/// </summary>
internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
	private readonly PermissionManager _permissionManager;
	private readonly UserManager<User> _userManager;


	public PermissionAuthorizationHandler(PermissionManager permissionManager, UserManager<User> userManager)
	{
		_permissionManager = permissionManager;
		_userManager = userManager;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
	{
		if (context.User != null)
		{
			var user = await _userManager.GetUserAsync(context.User);
			if (user != null &&
				(await _userManager.IsInRoleAsync(user, CoreRoles.SuperAdminRoleName) ||
				_permissionManager.HasPermission(context.User, requirement.Permission)))
				context.Succeed(requirement);
		}
	}
}
