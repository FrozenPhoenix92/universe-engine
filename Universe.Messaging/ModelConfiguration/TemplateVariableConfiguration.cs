using Universe.Messaging.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Messaging.ModelConfiguration;

public class TemplateVariableConfiguration : IEntityTypeConfiguration<TemplateVariable>
{
	public void Configure(EntityTypeBuilder<TemplateVariable> builder)
	{
		builder.HasIndex(x => x.Name).IsUnique();

		builder.Property(x => x.Name).HasMaxLength(100);
	}
}
