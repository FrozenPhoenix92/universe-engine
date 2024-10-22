using Universe.Core.Audit;
using Universe.Core.Data;
using Universe.Core.ModuleBinding;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Universe.Core.Membership.Data;

public abstract class AuditableIdentityDbContextCore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	: IdentityDbContextCore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IDbContext
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserClaim : IdentityUserClaim<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TUserLogin : IdentityUserLogin<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
		where TUserToken : IdentityUserToken<TKey>
{
	private readonly IAuditService? _auditService;


	protected AuditableIdentityDbContextCore(DbContextOptions options, IDictionary<string, object>? features = null) : base(options) =>
		_auditService = (IAuditService?)features?.Values.SingleOrDefault(x => x is IAuditService);


	/// <summary>
	/// Возвращает уникальное значение контекста, с помощью которого осуществляется поиск и применение внешних настроек моделей из других модулей.
	/// </summary>
	public string ModuleBindingId => CoreStaticData.MainDataContextBindingId;


	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		ConfigurableFromModulesDbContextCore.ApplyModelsConfiguration(builder, ModuleBindingId);
	}

	public override int SaveChanges()
	{
		if (_auditService is null) return base.SaveChanges();

		_auditService.OnBeforeSaveChanges(ChangeTracker.Entries().ToArray());
		var result = base.SaveChanges();
		_auditService.OnAfterSaveChanges();
		return result;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		if (_auditService is null) return await base.SaveChangesAsync(cancellationToken);

		_auditService.OnBeforeSaveChanges(ChangeTracker.Entries().ToArray());
		var result = await base.SaveChangesAsync(cancellationToken);
		await _auditService.OnAfterSaveChangesAsync(cancellationToken);
		return result;
	}
}

