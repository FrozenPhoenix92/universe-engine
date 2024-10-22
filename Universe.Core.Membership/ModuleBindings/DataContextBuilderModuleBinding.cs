using Universe.Core.AppConfiguration.Model;
using Universe.Core.Audit;
using Universe.Core.Membership.Model;
using Universe.Core.ModuleBinding.Bindings;

using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Membership.ModuleBindings;

public class MainDataContextBuilderModuleBinding : IDataContextBuilderModuleBinding
{
	public string ModuleBindingId => CoreStaticData.MainDataContextBindingId;

	public void ConfigureModels(ModelBuilder builder)
	{
		builder.Entity<PasswordChange>();
		builder.Entity<Permission>();
		builder.Entity<RolePermission>();
		builder.Entity<UserPermission>();
	}
}

public class AuditDataContextBuilderModuleBinding : IDataContextBuilderModuleBinding
{
	public string ModuleBindingId => StaticData.AuditDataContextBindingId;

	public void ConfigureModels(ModelBuilder builder)
	{
		builder.Entity<SignInAudit>();
	}
}
