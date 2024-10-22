using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Audit;
using Universe.Core.Configuration;
using Universe.Core.Exceptions;
using Universe.Core.Infrastructure;
using Universe.Core.ModuleBinding.Bindings;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Universe.Core.Common.ModuleBinding;

public class AuditModuleBindings : IDataContextModuleBinding, IDbCreationModuleBinding
{
	public void AddDataContext(IServiceCollection services, IConfiguration configuration)
	{
		var connectionSettings = configuration.GetSection(StaticData.DatabaseConnectionSettingsConfigurationSectionName)
			.Get<DatabaseConnectionSettings>();

		if (connectionSettings is null)
			throw new MissedConfigurationSectionException(StaticData.DatabaseConnectionSettingsConfigurationSectionName);

		if (connectionSettings.DatabaseType is null)
			throw new MissedConfigurationSectionException(
				$"{StaticData.DatabaseConnectionSettingsConfigurationSectionName}.{nameof(DatabaseConnectionSettings.DatabaseType)}");

		if (string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
			throw new MissedConfigurationSectionException(
				$"{StaticData.DatabaseConnectionSettingsConfigurationSectionName}.{nameof(DatabaseConnectionSettings.ConnectionString)}");

		switch (connectionSettings.DatabaseType) {
			case DatabaseType.MySql:
				services.AddDbContext<AuditDataContext>(options =>
					options.UseMySql(
						connectionSettings.ConnectionString,
						ServerVersion.AutoDetect(connectionSettings.ConnectionString),
						builder => builder.EnableRetryOnFailure(
							connectionSettings.MaxRetryCount,
							TimeSpan.FromSeconds(connectionSettings.MaxRetryDelay),
							connectionSettings.ErrorNumbersToAdd)));
				break;
			case DatabaseType.MsSql:
				services.AddDbContext<AuditDataContext>(options =>
					options.UseSqlServer(
						connectionSettings.ConnectionString,
						builder => builder.EnableRetryOnFailure(
							connectionSettings.MaxRetryCount,
							TimeSpan.FromSeconds(connectionSettings.MaxRetryDelay),
							connectionSettings.ErrorNumbersToAdd)));
				break;
			default: throw new NotImplementedException("Другие типы БД пока не реализованы.");
		}

		services.AddScoped<IAuditDataContext, AuditDataContext>();
		services.AddScoped<IDataService<ChangeLog, int, IAuditDataContext>, DataService<ChangeLog, int, IAuditDataContext>>();
	}

	public void Create(IServiceScope serviceScope)
	{
		var auditContext = serviceScope.ServiceProvider.GetService<IAuditDataContext>();

		if (auditContext is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IAuditDataContext));

		auditContext.Database.EnsureCreated();
	}
}
