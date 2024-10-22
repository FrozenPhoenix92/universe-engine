using Universe.Core.Data;
using Universe.Core.ModuleBinding;

using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Audit;

public interface IAuditDataContext : IDbContext
{
    DbSet<ChangeLog>? ChangeLogs { get; set; }

    DbSet<ChangeLogItem>? ChangeLogItems { get; set; }
}

public class AuditDataContext : ConfigurableFromModulesDbContextCore, IAuditDataContext
{
    public AuditDataContext(DbContextOptions<AuditDataContext> options) : base(options)
    {
	}


	public DbSet<ChangeLog>? ChangeLogs { get; set; }

	public DbSet<ChangeLogItem>? ChangeLogItems { get; set; }

	/// <summary>
	/// Возвращает уникальное значение контекста, с помощью которого осуществляется поиск и применение внешних настроек моделей из других модулей.
	/// </summary>
	public override string ModuleBindingId => StaticData.AuditDataContextBindingId;
}
