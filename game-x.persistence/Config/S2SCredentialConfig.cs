using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class S2SCredentialConfig : IEntityTypeConfiguration<S2SCredential>
{
    public void Configure(EntityTypeBuilder<S2SCredential> builder)
    {
        builder.ToTable("s2s_credentials");

        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id)
            .IsRequired();

        builder.Property(sc => sc.SettingId)
            .IsRequired();

        builder.Property(sc => sc.KeyId)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(sc => sc.Direction)
            .IsRequired();

        builder.Property(sc => sc.AuthMethod)
            .IsRequired();

        builder.Property(sc => sc.UsageScope)
            .IsRequired();

        builder.Property(sc => sc.Status)
            .IsRequired();

        builder.Property(sc => sc.ActivatedAt)
            .IsRequired();

        builder.Property(sc => sc.ExpiredAt)
            .IsRequired();

        builder.Property(sc => sc.CreatedAt)
            .IsRequired();

        builder.Property(sc => sc.UpdatedAt)
            .IsRequired();

        builder.HasOne(sc => sc.ClientSetting)
            .WithMany(scs => scs.Credentials)
            .HasForeignKey(sc => sc.SettingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sc => sc.KeyId).IsUnique();
    }
}
