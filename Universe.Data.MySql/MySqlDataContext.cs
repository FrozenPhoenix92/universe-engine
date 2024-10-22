using Universe.Core.Audit;

using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Universe.Data.MySql;

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
///     dotnet ef migrations add MIGRATION_NAME -p ../Universe.Data.MySql -s ./ -c MySqlDataContext
/// </code>
/// Apply migrations:
/// <code>
///     dotnet ef database update -p ../Universe.Data.MySql -s ./ -c MySqlDataContext
/// </code>
public class MySqlDataContext : DataContext
{
    public MySqlDataContext(DbContextOptions<MySqlDataContext> options, IAuditService? auditService = null)
        : base(options, auditService == null ? null : new Dictionary<string, object> { [nameof(IAuditService)] = auditService }) 
    {

    }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.HasCharSet(CharSet.Utf8Mb4, false);
		builder.UseGuidCollation(string.Empty);
		base.OnModelCreating(builder);
	}
}