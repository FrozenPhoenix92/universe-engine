using Universe.Core.Membership.Model;
using Microsoft.EntityFrameworkCore;
using Universe.Core.Data;
using Universe.Core.Membership.Authorization;
using Universe.Core.ModuleBinding;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Universe.Core.Exceptions;

namespace Universe.Core.Common.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private static async Task SeedPermissions(
			PermissionManager permissionManager,
			RoleManager<Role> roleManager,
			IDbContext context,
			Role superAdminRole,
			IEnumerable<IRolePermissionProvider> rolePermissionProviders)
	{
		var permissionProviders = AssembliesManager.GetInstances<IPermissionProvider>();
		var allPermissionNames = permissionProviders
			.SelectMany(x => x.GetType().GetFields(BindingFlags.Static | BindingFlags.Public)
				.Select(x => x.GetValue(null)?.ToString()));

		var existingPermissions = await context.Set<Permission>()
			.Where(x => allPermissionNames.Contains(x.Name))
			.ToListAsync();

		var unexistingPermissionNames = allPermissionNames
			.Where(x => existingPermissions.All(y => y.Name?.Equals(x, StringComparison.InvariantCultureIgnoreCase) is false));

		if (unexistingPermissionNames.Any())
		{
			var newPermissions = unexistingPermissionNames.Select(x => new Permission { Name = x }).ToList();
			await context.Set<Permission>().AddRangeAsync(newPermissions);

			var allRoles = await roleManager.Roles.ToListAsync();
			var fullRolesPermissionNamesMap = rolePermissionProviders
				.SelectMany(x => x.RolesPermissionsMap)
				.Where(x => !x.Key.Equals(superAdminRole.Name, StringComparison.InvariantCultureIgnoreCase))
				.ToDictionary(
					x => allRoles.SingleOrDefault(y => y.Name.Equals(x.Key, StringComparison.InvariantCultureIgnoreCase))
						?? throw new StartupCriticalException("В качестве индекса одного из словарей ролей и их наборов прав указано имя несуществующей роли."),
					x => x.Value);

			foreach (var newPermission in newPermissions) 
			{
				foreach (var rolePermissionsMapItem in fullRolesPermissionNamesMap.Where(x => x.Value.Contains(newPermission.Name)))
				{
					await permissionManager.AddPermissionForRole(rolePermissionsMapItem.Key, newPermission);
				}
			}
		}

		await SeedSuperAdminPermissions(permissionManager, context, superAdminRole, permissionProviders.OfType<ISuperAdminPermissions>());
	}

	private static async Task SeedSuperAdminPermissions(
		PermissionManager permissionManager,
		IDbContext context,
		Role superAdminRole,
		IEnumerable<ISuperAdminPermissions> superAdminPermissionProviders)
	{
		var superAdminPermissionNames = superAdminPermissionProviders
			.SelectMany(x => x.GetType().GetFields(BindingFlags.Static | BindingFlags.Public)
				.Select(x => x.GetValue(null)?.ToString()));

		var superAdminExistingPermissions = await context.Set<Permission>()
			.Where(x => superAdminPermissionNames.Contains(x.Name))
			.ToListAsync();

		var superAdminPermissions = await permissionManager.GetRolePermissions(superAdminRole);
		foreach (var permission in superAdminExistingPermissions)
		{
			if (superAdminPermissions.All(x => x?.Name?.Equals(permission.Name, StringComparison.InvariantCultureIgnoreCase) is false))
			{
				await permissionManager.AddPermissionForRole(superAdminRole, permission);
			}
		}
	}
}
