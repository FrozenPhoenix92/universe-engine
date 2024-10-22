using Universe.Core.Common.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Common.ModelConfiguration;

public class IpAddressConstraintConfiguration : IEntityTypeConfiguration<IpAddressConstraint>
{
	public void Configure(EntityTypeBuilder<IpAddressConstraint> builder)
	{
		builder.Property(x => x.AddressesRangeStart).HasMaxLength(15);
		builder.Property(x => x.AddressesRangeEnd).HasMaxLength(15);
		builder.Property(x => x.ContainingUrlPart).HasColumnType("text");
	}
}
