using System.Reflection;

using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Authorization;
using Universe.Core.Membership.Model;
using Universe.Core.Membership.Services;
using Universe.Core.ModuleBinding;
using Universe.Core.ModuleBinding.Bindings;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.Common.ModuleBinding;

public partial class InitialDataModuleBinding : IInitialDataModuleBinding
{
	public int Order => 20;

	public async Task EnsureInitialData(IServiceScope serviceScope)
	{
		var context = serviceScope.ServiceProvider.GetService<IDbContext>();
		if (context is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IDbContext));

		var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
		if (roleManager is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(RoleManager<Role>));

		var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
		if (userManager is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(UserManager<User>));

		var permissionManager = serviceScope.ServiceProvider.GetService<PermissionManager>();
		if (permissionManager is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(PermissionManager));

		var securityService = serviceScope.ServiceProvider.GetService<ISecurityService>();
		if (securityService is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(ISecurityService));


		var rolePermissionProviders = AssembliesManager.GetInstances<IRolePermissionProvider>();

		var superAdminRole = await SeedRoles(roleManager, permissionManager, rolePermissionProviders);
		await SeedAppSettingsSet(context);
		await SeedEmailTemplates(context);
		await SeedPermissions(permissionManager, roleManager, context, superAdminRole, rolePermissionProviders);
	}
}
