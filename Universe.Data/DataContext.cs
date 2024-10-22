using Universe.Core.Membership.Data;
using Universe.Core.Membership.Model;
using Universe.Core.ModuleBinding;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Universe.Data;

public abstract class DataContext :
	AuditableIdentityDbContextCore<
		User,
		Role,
		Guid,
		IdentityUserClaim<Guid>,
		UserRole,
		IdentityUserLogin<Guid>,
		IdentityRoleClaim<Guid>,
		IdentityUserToken<Guid>>,
	IDataContext
{
	public DataContext(DbContextOptions options, IDictionary<string, object>? features = null) : base(options, features)
	{
	}


	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		foreach (var assembly in AssembliesManager.GetProjectAssemblies())
		{
			builder.ApplyConfigurationsFromAssembly(assembly);
		}
	}
}