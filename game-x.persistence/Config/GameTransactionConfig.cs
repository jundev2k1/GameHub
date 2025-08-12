using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GameTransactionConfig : IEntityTypeConfiguration<GameTransaction>
{
    public void Configure(EntityTypeBuilder<GameTransaction> builder)
    {
        builder.ToTable("game_transactions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Sno).IsUnique();
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Sno)
            .HasColumnName("sno")
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.GamePlatform)
            .HasColumnName("game_platform")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasDefaultValue(GameTransactionStatus.Pending)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder.Property(x => x.Note)
            .HasColumnName("note");

        builder.HasOne(x => x.User)
            .WithMany(u => u.GameTransactions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
