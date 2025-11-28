using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class AppSettingConfig : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("app_settings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(512)
            .HasDefaultValue(string.Empty);

        builder.Property(s => s.IsEditable)
            .IsRequired();

        builder.HasIndex(s => s.Key).IsUnique();
    }
}
