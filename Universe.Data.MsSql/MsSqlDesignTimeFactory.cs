using Universe.Core.Data;

using Microsoft.EntityFrameworkCore.Design;

namespace Universe.Data.MsSql;

public class MsSqlDesignTimeFactory : IDesignTimeDbContextFactory<MsSqlDataContext>
{
	public MsSqlDataContext CreateDbContext(string[] args) =>
		DesignTimeDataContextFactory.CreateForMsSql<MsSqlDataContext>(DataConfigurationSectionNames.MainDatabaseConnectionSettings);
}
