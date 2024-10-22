using Universe.Messaging.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Messaging.ModelConfiguration;

public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
	public void Configure(EntityTypeBuilder<EmailTemplate> builder)
	{
		builder.HasIndex(x => x.Code).IsUnique();

		builder.Property(x => x.Body).HasColumnType("TEXT");
		builder.Property(x => x.Code).HasMaxLength(100);
		builder.Property(x => x.From).HasMaxLength(250);
		builder.Property(x => x.Subject).HasMaxLength(500);
		builder.Property(x => x.Title).HasMaxLength(200);
	}
}
