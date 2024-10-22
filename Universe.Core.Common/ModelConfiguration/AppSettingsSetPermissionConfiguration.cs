using Universe.Core.Common.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Common.ModelConfiguration;

public class AppSettingsSetPermissionConfiguration : IEntityTypeConfiguration<AppSettingsSetPermission>
{
	public void Configure(EntityTypeBuilder<AppSettingsSetPermission> builder)
	{
		builder.HasOne(x => x.Permission)
			.WithMany()
			.HasForeignKey(x => x.PermissionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.AppSettingsSet)
			.WithMany(x => x.Permissions)
			.HasForeignKey(x => x.AppSettingsSetId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasKey(x => new { x.PermissionId, x.AppSettingsSetId });
	}
}
