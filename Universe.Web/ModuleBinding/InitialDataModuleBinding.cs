using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Authorization;
using Universe.Core.Membership.Model;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Files.Configuration;
using Universe.Web.Configuration;
using Universe.Web.Filters;
using Universe.Web.Model;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Universe.Web.ModuleBinding;

public partial class InitialDataModuleBinding : IInitialDataModuleBinding
{
	public int Order => 30;

	public async Task EnsureInitialData(IServiceScope serviceScope)
	{
		var webHostingEnvironment = serviceScope.ServiceProvider.GetService<IWebHostEnvironment>();
		if (webHostingEnvironment is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IWebHostEnvironment));

		var environmentSettings = serviceScope.ServiceProvider.GetService<IOptions<EnvironmentSettings>>()?.Value;
		if (environmentSettings is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IOptions<EnvironmentSettings>));
		
		var fileStorageSettings = serviceScope.ServiceProvider.GetService<IOptions<FileStorageSettings>>()?.Value;
		if (fileStorageSettings is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IOptions<FileStorageSettings>));

		var context = serviceScope.ServiceProvider.GetService<IDbContext>();
		if (context is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IDbContext));

		var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
		if (roleManager is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(RoleManager<Role>));

		var permissionManager = serviceScope.ServiceProvider.GetService<PermissionManager>();
		if (permissionManager is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(PermissionManager));

		var path = Path.Combine(
			webHostingEnvironment.ContentRootPath,
			environmentSettings.DataFilesRootFolder,
			fileStorageSettings.Folder);
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		var adminRole = await SeedRoles(roleManager);
		await SeedEmailTemplates(context);
		await SeedTestData(context);

		foreach (var blockedClientId in await context.Set<Client>().Where(x => x.Blocked).Select(x => x.Id).ToListAsync())
		{
			BlockedClientFilterAttribute.DisabledClientIds.TryAdd(blockedClientId, true);
		}
	}
}