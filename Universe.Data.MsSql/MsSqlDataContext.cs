using Universe.Core.Audit;

using Microsoft.EntityFrameworkCore;

namespace Universe.Data.MsSql;

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
///     dotnet ef migrations add MIGRATION_NAME -p ../Universe.Data.MsSql -s ./ -c MsSqlDataContext
/// </code>
/// Apply migrations:
/// <code>
///     dotnet ef database update -p ../Universe.Data.MsSql -s ./ -c MsSqlDataContext
/// </code>
public class MsSqlDataContext : DataContext
{
	public MsSqlDataContext(DbContextOptions<MsSqlDataContext> options, IAuditService? auditService = null)
		: base(options, auditService == null ? null : new Dictionary<string, object> { [nameof(IAuditService)] = auditService })
	{

	}
}