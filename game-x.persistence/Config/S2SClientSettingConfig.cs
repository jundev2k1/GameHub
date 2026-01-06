using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class S2SClientSettingConfig : IEntityTypeConfiguration<S2SClientSetting>
{
    public void Configure(EntityTypeBuilder<S2SClientSetting> builder)
    {
        builder.ToTable("s2s_client_settings");

        builder.HasKey(scs => scs.Id);
        builder.Property(scs => scs.Id)
            .IsRequired();

        builder.Property(scs => scs.ClientId)
            .IsRequired();

        builder.Property(scs => scs.AppCode)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(scs => scs.AppName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(scs => scs.IsActive)
            .IsRequired();

        builder.Property(scs => scs.Host)
            .IsRequired();

        builder.Property(scs => scs.AllowedIps)
            .IsRequired()
            .HasConversion(vo => vo.Value, v => AllowedIp.Of(v));

        builder.Property(scs => scs.Notes)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(scs => scs.CreatedAt)
            .IsRequired();

        builder.Property(scs => scs.UpdatedAt)
            .IsRequired();

        builder.HasOne(scs => scs.Client)
            .WithMany(sc => sc.Settings)
            .HasForeignKey(scs => scs.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
