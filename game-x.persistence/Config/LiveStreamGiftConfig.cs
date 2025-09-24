using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamGiftConfig : IEntityTypeConfiguration<LiveStreamGift>
{
    public void Configure(EntityTypeBuilder<LiveStreamGift> builder)
    {
        builder.ToTable("livestream_gifts");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(g => g.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(g => g.PublicId)
            .IsUnique();

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(g => g.Notes)
            .HasMaxLength(4000);

        builder.Property(g => g.CoinCost)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(g => g.Priority)
            .IsRequired();

        builder.Property(g => g.IsActive)
            .IsRequired();

        builder.Property(g => g.IsDeleted)
            .IsRequired();

        builder.Property(g => g.IconId)
            .IsRequired(false);

        builder.Property(g => g.AnimationId)
            .IsRequired(false);

        builder.HasOne(g => g.Icon)
            .WithMany()
            .HasForeignKey(g => g.IconId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(g => g.Animation)
            .WithMany()
            .HasForeignKey(g => g.AnimationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
