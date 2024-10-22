using Universe.Core.Configuration;
using Universe.Core.Data;
using Universe.Core.Membership.Extensions;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Data.MsSql;

public static class StartupExtensions
{
	public static void AddMsSqlMainContext(this IServiceCollection services, DatabaseConnectionSettings connectionSettings)
	{
		VariablesChecker.CheckIsNotNull(connectionSettings, nameof(connectionSettings));
		VariablesChecker.CheckIsNotNull(connectionSettings.DatabaseType, nameof(connectionSettings.DatabaseType));
		VariablesChecker.CheckIsNotNullOrEmpty(connectionSettings.ConnectionString, nameof(connectionSettings.ConnectionString));

		services.AddDbContext<MsSqlDataContext>(options => options.UseSqlServer(connectionSettings.ConnectionString));

		services.AddScoped<IDataContext, MsSqlDataContext>();
		services.AddScoped<IDbContext, MsSqlDataContext>();
	}

	public static void AddMsSqlIdentity(this IServiceCollection services, IConfiguration configuration) =>
		services.AddIdentity<MsSqlDataContext>(configuration);
}
