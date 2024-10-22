using Universe.Web.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Web.ModelConfiguration;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
	public void Configure(EntityTypeBuilder<OrderLine> builder)
	{
		builder.HasOne(x => x.Product)
			.WithMany()
			.HasForeignKey(x => x.ProductId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.Order)
			.WithMany(x => x.OrderLines)
			.HasForeignKey(x => x.OrderId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
