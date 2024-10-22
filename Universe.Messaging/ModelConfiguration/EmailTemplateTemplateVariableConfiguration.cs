using Universe.Messaging.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Messaging.ModelConfiguration;

public class EmailTemplateTemplateVariableConfiguration : IEntityTypeConfiguration<EmailTemplateTemplateVariable>
{
	public void Configure(EntityTypeBuilder<EmailTemplateTemplateVariable> builder)
	{
		builder.HasKey(x => new { x.EmailTemplateId, x.TemplateVariableId });

		builder.HasOne(x => x.EmailTemplate)
			.WithMany(x => x.EmailTemplateTemplateVariables)
			.HasForeignKey(x => x.EmailTemplateId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.TemplateVariable)
			.WithMany(x => x.EmailTemplateTemplateVariables)
			.HasForeignKey(x => x.TemplateVariableId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
