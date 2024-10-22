using Universe.Core.Data;

using Microsoft.EntityFrameworkCore.Design;

namespace Universe.Data.PostgreSql;

public class PostrgeSqlDesignTimeFactory : IDesignTimeDbContextFactory<PostgreSqlDataContext>
{
	public PostgreSqlDataContext CreateDbContext(string[] args) =>
		DesignTimeDataContextFactory.CreateForPostgreSql<PostgreSqlDataContext>(DataConfigurationSectionNames.MainDatabaseConnectionSettings);
}
