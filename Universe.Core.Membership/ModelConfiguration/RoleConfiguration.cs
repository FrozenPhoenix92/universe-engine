using Universe.Core.Membership.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Membership.ModelConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{ 
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Id).HasMaxLength(50).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.NormalizedName).HasMaxLength(256);
        builder.Property(x => x.ConcurrencyStamp).HasMaxLength(256);
    }
}
