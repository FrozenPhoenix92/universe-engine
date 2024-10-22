using Universe.Core.Exceptions;
using Universe.Core.AppConfiguration.Services;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;
using Universe.Core.Data;
using Universe.Core.Common.Model;
using Universe.Core.AppConfiguration;
using Universe.Core.Membership.Model;
using Microsoft.EntityFrameworkCore;
using Universe.Core.Membership;

namespace Universe.Core.Common.Extensions;

/// <summary>
/// Ресширения для процесса старта приложения.
/// </summary>
public static class StartupExtensions
{
	/// <summary>
	/// Инициализирует наборы настроек конфигурации системы на основе данных в хранилище.
	/// </summary>
	public static void InitializeAppConfiguration(this IServiceScope serviceScope)
	{
		var context = serviceScope.ServiceProvider.GetService<IDbContext>();

		if (context is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IDbContext));

		var appSettingsService = serviceScope.ServiceProvider.GetService<IAppSettingsService>();

		if (appSettingsService is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IAppSettingsService));

		appSettingsService.Initialize<AppSettingsSet>(context, serviceScope.ServiceProvider).Wait();

		AppState.SuperAdminExists = context.Set<UserRole>()
			.Include(x => x.Role)
			.Any(x => x.Role.Name == CoreRoles.SuperAdminRoleName);
	}
}