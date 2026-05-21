using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class NavigationItemConfig : IEntityTypeConfiguration<NavigationItem>
{
    public void Configure(EntityTypeBuilder<NavigationItem> builder)
    {
        builder.ToTable("navigation_items");

        builder.HasKey(ni => ni.Id);

        builder.Property(ni => ni.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(ni => ni.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(ni => ni.Title)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(ni => ni.Slug)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(ni => ni.TargetType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(ni => ni.TargetId)
            .IsRequired(false);

        builder.Property(ni => ni.CustomUrl)
            .IsRequired()
            .HasMaxLength(2000)
            .HasDefaultValue(string.Empty);

        builder.Property(ni => ni.IconId)
            .IsRequired(false);

        builder.Property(ni => ni.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ni => ni.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasOne(ni => ni.Icon)
            .WithMany()
            .HasForeignKey(ni => ni.IconId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ni => ni.Translations)
            .WithOne(t => t.NavigationItem)
            .HasForeignKey(t => t.NavigationItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ni => ni.PublicId).IsUnique();

        builder.HasIndex(ni => ni.Slug);
    }
}
