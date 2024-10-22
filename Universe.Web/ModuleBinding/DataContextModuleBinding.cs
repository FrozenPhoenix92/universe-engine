using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Configuration;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Data;
using Universe.Data.MySql;
using Universe.Data.PostgreSql;
using Universe.Data.MsSql;

namespace Universe.Web.ModuleBinding;

public class DataContextModuleBinding : IDataContextModuleBinding
{
	public void AddDataContext(IServiceCollection services, IConfiguration configuration)
	{
		var connectionSettings = configuration.GetSection(nameof(DataConfigurationSectionNames.MainDatabaseConnectionSettings))
			?.Get<DatabaseConnectionSettings>();

		if (connectionSettings is null)
			throw new MissedConfigurationSectionException(DataConfigurationSectionNames.MainDatabaseConnectionSettings);
		
		if (connectionSettings.DatabaseType is null)
			throw new MissedConfigurationSectionException(
				$"{DataConfigurationSectionNames.MainDatabaseConnectionSettings}.{nameof(DatabaseConnectionSettings.DatabaseType)}");

		if (string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
			throw new MissedConfigurationSectionException(
				$"{DataConfigurationSectionNames.MainDatabaseConnectionSettings}.{nameof(DatabaseConnectionSettings.ConnectionString)}");

		switch (connectionSettings.DatabaseType)
		{
			case DatabaseType.MySql:
				services.AddMySqlMainContext(connectionSettings);
				services.AddMySqlIdentity(configuration);
				break;
			case DatabaseType.PostgreSql:
				services.AddPostgreSqlMainContext(connectionSettings);
				services.AddPostgreSqlIdentity(configuration);
				break;
			case DatabaseType.MsSql:
				services.AddMsSqlMainContext(connectionSettings);
				services.AddMsSqlIdentity(configuration);
				break;
			default: throw new NotImplementedException("Другие типы БД пока не реализованы.");
		}
	}
}
