using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class AppUserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.Nickname)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.CountryCode)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(UserStatus.Active);
    }
}
