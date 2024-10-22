using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Universe.Web.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private static async Task<Role> SeedRoles(RoleManager<Role> roleManager)
	{
		var adminRole = await roleManager.Roles.SingleOrDefaultAsync(x => x.Name == ProjectRoles.AdminRoleName);
		if (adminRole is null)
		{
			adminRole = new Role { Name = ProjectRoles.AdminRoleName };
			await roleManager.CreateAsync(adminRole);
		}

		return adminRole;
	}
}