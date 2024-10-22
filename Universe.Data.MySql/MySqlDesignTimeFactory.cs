using Universe.Core.Data;

using Microsoft.EntityFrameworkCore.Design;

namespace Universe.Data.MySql;

public class MySqlDesignTimeFactory : IDesignTimeDbContextFactory<MySqlDataContext>
{
	public MySqlDataContext CreateDbContext(string[] args) =>
		DesignTimeDataContextFactory.CreateForMySql<MySqlDataContext>(DataConfigurationSectionNames.MainDatabaseConnectionSettings);
}
