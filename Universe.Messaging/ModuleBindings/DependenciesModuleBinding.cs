using BBWM.Messages;

using Universe.Core.ModuleBinding.Bindings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Universe.Messaging.ModuleBindings;

public class DependenciesModuleBinding : IDependenciesModuleBinding
{
	public void AddDependencies(IServiceCollection services, IConfiguration configuration)
	{
		var emailSettingsSection = configuration.GetSection(nameof(EmailMessagingSettings));
		var emailSettings = emailSettingsSection?.Get<EmailMessagingSettings>();

		if (emailSettingsSection is null || emailSettings is null)
		{
			Log.Warning($"Не задана секция конфигурации '{nameof(EmailMessagingSettings)}'.");
		}
		else
		{
			services.Configure<EmailMessagingSettings>(emailSettingsSection);
		}
	}
}
