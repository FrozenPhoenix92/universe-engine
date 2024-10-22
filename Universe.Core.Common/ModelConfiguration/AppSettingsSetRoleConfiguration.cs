using Universe.Core.Common.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Common.ModelConfiguration;

public class AppSettingsSetRoleConfiguration : IEntityTypeConfiguration<AppSettingsSetRole>
{
	public void Configure(EntityTypeBuilder<AppSettingsSetRole> builder)
	{
		builder.HasOne(x => x.Role)
			.WithMany()
			.HasForeignKey(x => x.RoleId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.AppSettingsSet)
			.WithMany(x => x.Roles)
			.HasForeignKey(x => x.AppSettingsSetId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasKey(x => new { x.RoleId, x.AppSettingsSetId });
	}
}
