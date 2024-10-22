using Universe.Core.Audit;

using Microsoft.EntityFrameworkCore;

namespace Universe.Data.PostgreSql;

/// <summary>
/// Main data context of the project.
/// </summary>
/// <remarks>
/// <para>
/// Run these commands in ./Universe.Web folder.
/// </para>
/// Move to the folder:
/// cd ./Universe.Web
/// Add migration:
/// <code>
///     dotnet ef migrations add MIGRATION_NAME -p ../Universe.Data.PostgreSql -s ./ -c PostgreSqlDataContext
/// </code>
/// Apply migrations:
/// <code>
///     dotnet ef database update -p ../Universe.Data.PostgreSql -s ./ -c PostgreSqlDataContext
/// </code>
public class PostgreSqlDataContext : DataContext
{
	public PostgreSqlDataContext(DbContextOptions<PostgreSqlDataContext> options, IAuditService? auditService = null)
		: base(options, auditService == null ? null : new Dictionary<string, object> { [nameof(IAuditService)] = auditService })
	{

	}
}