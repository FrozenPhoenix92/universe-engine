using Universe.Web.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Web.ModelConfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.HasOne(x => x.Customer)
			.WithMany()
			.HasForeignKey(x => x.CustomerId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(x => x.Status).HasConversion<string>();
	}
}
