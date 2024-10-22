using Universe.Core.Configuration;
using Universe.Core.Data;
using Universe.Core.Membership.Extensions;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Data.PostgreSql;

public static class StartupExtensions
{
	public static void AddPostgreSqlMainContext(this IServiceCollection services, DatabaseConnectionSettings connectionSettings)
	{
		VariablesChecker.CheckIsNotNull(connectionSettings, nameof(connectionSettings));
		VariablesChecker.CheckIsNotNull(connectionSettings.DatabaseType, nameof(connectionSettings.DatabaseType));
		VariablesChecker.CheckIsNotNullOrEmpty(connectionSettings.ConnectionString, nameof(connectionSettings.ConnectionString));

		services.AddDbContext<PostgreSqlDataContext>(options => options.UseNpgsql(connectionSettings.ConnectionString));

		services.AddScoped<IDataContext, PostgreSqlDataContext>();
		services.AddScoped<IDbContext, PostgreSqlDataContext>();
	}

	public static void AddPostgreSqlIdentity(this IServiceCollection services, IConfiguration configuration) =>
		services.AddIdentity<PostgreSqlDataContext>(configuration);
}
