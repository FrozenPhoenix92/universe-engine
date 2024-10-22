using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Infrastructure;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Web.Configuration;
using Universe.Web.Model;

namespace Universe.Web.ModuleBinding;

public class DependenciesModuleBinding : IDependenciesModuleBinding
{
	public void AddDependencies(IServiceCollection services, IConfiguration configuration)
	{
		AddCoreInjections(services, configuration);
		AddProjectInjections(services, configuration);
	}


	private static void AddCoreInjections(IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		services.AddHttpClient();
	}

	private static void AddProjectInjections(IServiceCollection services, IConfiguration configuration)
	{
		ConfigureOptions(services, configuration);
		AddServices(services, configuration);
	}

	private static void AddServices(IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IDataService<Client, Guid>, DataService<Client, Guid>>();
	}

	private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
	{
		var environmentSettingsSection = configuration.GetSection(nameof(EnvironmentSettings));
		var environmentSettings = environmentSettingsSection?.Get<EnvironmentSettings>();
		if (environmentSettingsSection is null || environmentSettings is null)
			throw new MissedConfigurationSectionException(nameof(EnvironmentSettings));
		services.Configure<EnvironmentSettings>(environmentSettingsSection);

		var clientApplicationSettingsSection = configuration.GetSection(nameof(ClientApplicationSettings));
		var clientApplicationSettings = clientApplicationSettingsSection?.Get<List<ClientApplicationSettings>>();
		if (clientApplicationSettingsSection is null || clientApplicationSettings is null)
			throw new MissedConfigurationSectionException(nameof(ClientApplicationSettings));
		services.Configure<List<ClientApplicationSettings>>(clientApplicationSettingsSection);
	}
}
