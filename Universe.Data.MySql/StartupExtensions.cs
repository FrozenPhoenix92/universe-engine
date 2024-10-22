using Universe.Core.Configuration;
using Universe.Core.Data;
using Universe.Core.Membership.Extensions;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Data.MySql;

public static class StartupExtensions
{
	public static void AddMySqlMainContext(this IServiceCollection services, DatabaseConnectionSettings connectionSettings)
	{
		VariablesChecker.CheckIsNotNull(connectionSettings, nameof(connectionSettings));
		VariablesChecker.CheckIsNotNull(connectionSettings.DatabaseType, nameof(connectionSettings.DatabaseType));
		VariablesChecker.CheckIsNotNullOrEmpty(connectionSettings.ConnectionString, nameof(connectionSettings.ConnectionString));

		services.AddDbContext<MySqlDataContext>(options =>
			options.UseMySql(connectionSettings.ConnectionString,
							 ServerVersion.AutoDetect(connectionSettings.ConnectionString),
							 builder => builder.EnableRetryOnFailure(
								 connectionSettings.MaxRetryCount,
								 TimeSpan.FromSeconds(connectionSettings.MaxRetryDelay),
								 connectionSettings.ErrorNumbersToAdd)).EnableSensitiveDataLogging());

		services.AddScoped<IDataContext, MySqlDataContext>();
		services.AddScoped<IDbContext, MySqlDataContext>();
	}
	 
	public static void AddMySqlIdentity(this IServiceCollection services, IConfiguration configuration) =>
		services.AddIdentity<MySqlDataContext>(configuration);
}
