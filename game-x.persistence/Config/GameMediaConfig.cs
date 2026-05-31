using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GameMediaConfig : IEntityTypeConfiguration<GameMedia>
{
    public void Configure(EntityTypeBuilder<GameMedia> builder)
    {
        builder.ToTable("game_medias");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.GameId)
            .IsRequired();

        builder.Property(x => x.FileId)
            .IsRequired(false);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(x => x.Category)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.Note)
            .IsRequired(false)
            .HasMaxLength(4000);

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.GameId,
            x.Category,
            x.Priority
        });

        builder.HasOne(x => x.Game)
            .WithMany(g => g.GameMedias)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
