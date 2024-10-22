using Universe.Core.Membership.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Membership.ModelConfiguration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder) 
    {
        builder.Property(x => x.Id).HasMaxLength(50).ValueGeneratedOnAdd();

        builder.HasIndex(x => x.Name).IsUnique();
    }
}
