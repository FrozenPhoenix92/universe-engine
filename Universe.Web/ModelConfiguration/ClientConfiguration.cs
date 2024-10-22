using Universe.Web.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Web.ModelConfiguration;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
	public void Configure(EntityTypeBuilder<Client> builder)
	{
		builder.HasIndex(x => new { x.Email, x.AppCode }).IsUnique();

		builder.Property(x => x.Id).HasMaxLength(50).ValueGeneratedOnAdd();
		builder.Property(x => x.Email).HasMaxLength(100);
	}
}
