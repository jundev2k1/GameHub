using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(rt => rt.PublicId)
            .IsRequired();

        builder.Property(rt => rt.UserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(rt => rt.TokenHash)
            .HasMaxLength(88)
            .IsRequired();

        builder.Property(rt => rt.JwtId)
            .IsRequired();

        builder.Property(rt => rt.UserAgent)
            .HasMaxLength(512)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(rt => rt.IpAddress)
            .HasMaxLength(45)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(rt => rt.DeviceInfo)
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty);

        builder.Property(rt => rt.Location)
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty);

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Ignore(rt => rt.UpdatedAt);

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rt => rt.PublicId)
            .IsUnique();

        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();

        builder.HasIndex(rt => rt.JwtId);
    }
}
