using Universe.Files.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Messaging.ModelConfiguration;

public class FileDetailsConfiguration : IEntityTypeConfiguration<FileDetails>
{
	public void Configure(EntityTypeBuilder<FileDetails> builder)
	{
		builder.HasIndex(x => x.Name).IsUnique();

		builder.Property(x => x.Name).HasMaxLength(100);
		builder.Property(x => x.Extension).HasMaxLength(20);
		builder.Property(x => x.RelatedDataKey).HasMaxLength(150);
		builder.Property(x => x.RelatedOperationKey).HasMaxLength(150);
		builder.Property(x => x.RelatedResponsibleKey).HasMaxLength(150);
	}
}
