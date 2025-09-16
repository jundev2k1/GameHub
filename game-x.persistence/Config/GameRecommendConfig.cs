using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameRecommendConfig : IEntityTypeConfiguration<GameRecommend>
{
    public void Configure(EntityTypeBuilder<GameRecommend> builder)
    {
        builder.ToTable("game_recommends");

        builder.HasKey(gr => gr.Id);

        builder.Property(gr => gr.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gr => gr.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(gr => gr.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(gr => gr.Description)
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gr => gr.BannerId)
            .IsRequired(false);

        builder.HasOne(gr => gr.Banner)
            .WithMany()
            .HasForeignKey(gr => gr.BannerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(gr => gr.Status)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(PublishStatus.Draft);

        builder.Property(gr => gr.StartDate)
            .IsRequired(false);

        builder.Property(gr => gr.EndDate)
            .IsRequired(false);

        builder.HasIndex(gr => gr.PublicId)
            .IsUnique();

        builder.HasMany(gr => gr.Items)
            .WithOne(gri => gri.GameRecommend)
            .HasForeignKey(gri => gri.GameRecommendId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
