using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Universe.Web.Model;

namespace Universe.Web.ModelConfiguration;

public class ClientOperationConfirmationTokenConfiguration : IEntityTypeConfiguration<ClientOperationConfirmationToken>
{
	public void Configure(EntityTypeBuilder<ClientOperationConfirmationToken> builder)
	{
		builder.HasOne(x => x.Client)
			.WithMany()
			.HasForeignKey(x => x.ClientId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(x => x.Token).HasMaxLength(50);
		builder.Property(x => x.Operation).HasMaxLength(256);

		builder.HasIndex(x => new { x.ClientId, x.Operation }).IsUnique();
	}
}
