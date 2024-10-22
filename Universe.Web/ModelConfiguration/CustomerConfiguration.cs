using Universe.Web.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Web.ModelConfiguration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.Property(x => x.Address).HasMaxLength(250);
		builder.Property(x => x.Name).HasMaxLength(100);
		builder.Property(x => x.LastName).HasMaxLength(100);
		builder.Property(x => x.Photo).HasColumnType("text");
	}
}
