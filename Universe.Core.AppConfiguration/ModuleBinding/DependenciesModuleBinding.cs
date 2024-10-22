using Universe.Core.ModuleBinding.Bindings;
using Universe.Core.AppConfiguration.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Universe.Core.AppConfiguration.Configuration;
using Serilog;

namespace Universe.Core.AppConfiguration.ModuleBindings;

public class DependenciesModuleBinding : IDependenciesModuleBinding
{
	public void AddDependencies(IServiceCollection services, IConfiguration configuration)
	{
		var appSettingsSection = configuration.GetSection(nameof(AppSettings));
		var appSettings = appSettingsSection?.Get<AppSettings>();
		if (appSettingsSection is null || appSettings is null)
			Log.Warning($"Не задана секция конфигурации '{nameof(AppSettings)}'.");
		services.Configure<AppSettings>(appSettingsSection);

		services.AddSingleton<IAppSettingsService, AppSettingsService>();
	}
}