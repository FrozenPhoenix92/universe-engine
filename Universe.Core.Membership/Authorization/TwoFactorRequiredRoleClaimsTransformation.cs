using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Reflection;
using System.Security.Claims;

namespace Universe.Core.Membership.Authorization;

public class TwoFactorRequiredRoleClaimsTransformation : IClaimsTransformation
{
	private readonly RoleManager<Role> _roleManager;
	private readonly UserManager<User> _userManager;


	public TwoFactorRequiredRoleClaimsTransformation(RoleManager<Role> roleManager, UserManager<User> userManager)
	{
		_roleManager = roleManager;
		_userManager = userManager;
	}

	public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
	{
		/*if (principal.Identity?.IsAuthenticated is false || principal.Identity is not ClaimsIdentity claimsIdentity ||
			!principal.HasClaim(x => x.Type.Equals(CorePolicies.TwoFactorRequiredRolePolicyName, StringComparison.OrdinalIgnoreCase)) ||
			await _userManager.)
		{
			return principal;
		}



		var twoFactorRequiredRoles = await _roleManager.Roles.Where(x => x.TwoFactorRequired).Select(x => x.Name).ToListAsync();

		foreach (var role in twoFactorRequiredRoles)
		{
			if (principal.IsInRole(role))
			{
				claimsIdentity.AddClaim(
					new Claim(
						CorePolicies.TwoFactorRequiredRolePolicyName,
						"true",
						ClaimValueTypes.Boolean,
						Assembly.GetExecutingAssembly().FullName));
				break;
			}
		}*/

		return principal;
	}
}
