using Universe.Core.Membership.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Universe.Core.Membership.ModelConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.Id).HasMaxLength(50).ValueGeneratedOnAdd();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.SecurityStamp).HasMaxLength(256);
        builder.Property(x => x.PasswordHash).HasMaxLength(256);
        builder.Property(x => x.ConcurrencyStamp).HasMaxLength(256);
    }
}
