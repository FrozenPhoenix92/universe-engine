using Universe.Core.Membership.Model;
using Universe.Core.Membership;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Universe.Core.Membership.Authorization;

namespace Universe.Core.Common.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private static async Task<Role> SeedRoles(
		RoleManager<Role> roleManager,
		PermissionManager permissionManager,
		IEnumerable<IRolePermissionProvider> rolePermissionProviders)
	{
		var superAdminRole = await SeedSuperAdminRole(roleManager);

		foreach (var rolePermissionsPair in rolePermissionProviders.SelectMany(x => x.RolesPermissionsMap)
			.Where(x => !x.Key.Equals(superAdminRole.Name, StringComparison.InvariantCultureIgnoreCase)))
		{
			var role = await roleManager.Roles.SingleOrDefaultAsync(x => x.Name == rolePermissionsPair.Key);

			if (role == null)
			{
				role = new Role { Name = rolePermissionsPair.Key };
				await roleManager.CreateAsync(role);
			}

			var rolePermissions = await permissionManager.GetRolePermissions(role);

			foreach (var permissionName in rolePermissionsPair.Value
				.Where(x => rolePermissions.All(y => y?.Name?.Equals(x, StringComparison.InvariantCultureIgnoreCase) is false)))
			{
				var permission = await permissionManager.FindByNameAsync(permissionName);

				if (permission != null)
				{
					await permissionManager.AddPermissionForRole(role, permission);
				}
			}
		}

		return superAdminRole;
	}

	private static async Task<Role> SeedSuperAdminRole(RoleManager<Role> roleManager)
	{
		var role = await roleManager.Roles.SingleOrDefaultAsync(x => x.Name == CoreRoles.SuperAdminRoleName);

		if (role is not null) return role;

		role = new Role { Name = CoreRoles.SuperAdminRoleName };
		await roleManager.CreateAsync(role);

		return role;
	}
}
