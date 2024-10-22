using Universe.Core.Common.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Common.ModelConfiguration;

public class AppSettingsSetConfiguration : IEntityTypeConfiguration<AppSettingsSet>
{
	public void Configure(EntityTypeBuilder<AppSettingsSet> builder)
	{
		builder.Property(x => x.Name).HasMaxLength(50);
		builder.Property(x => x.Value).HasColumnType("text");
	}
}
