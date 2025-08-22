using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GameTransactionConfig : IEntityTypeConfiguration<GameTransaction>
{
    public void Configure(EntityTypeBuilder<GameTransaction> builder)
    {
        builder.ToTable("game_transactions");

        builder.HasKey(gt => gt.Id);

        builder.HasIndex(gt => gt.G598Sno).IsUnique();
        builder.HasIndex(gt => gt.PublicId).IsUnique();

        builder.Property(gt => gt.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gt => gt.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(gt => gt.G598Sno)
            .HasColumnName("g598_sno")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(gt => gt.UserId)
            .IsRequired();

        builder.Property(gt => gt.GamePlatformId)
            .IsRequired();

        builder.Property(gt => gt.Type)
            .HasConversion<short>()
            .IsRequired();

        builder.Property(gt => gt.Status)
            .HasColumnName("status")
            .HasConversion<short>()
            .IsRequired();
        
        builder.Property(gt => gt.Amount)
            .IsRequired();

        builder.Property(gt => gt.Note)
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.CryptoTokenId)
            .HasColumnName("crypto_token_id");

        builder.Property(gt => gt.Meta)
            .HasColumnName("meta")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");
        
        builder.HasOne(gt => gt.User)
            .WithMany(u => u.GameTransactions)
            .HasForeignKey(gt => gt.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(gt => gt.CryptoToken)
            .WithMany()
            .HasForeignKey(gt => gt.CryptoTokenId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(gt => gt.GamePlatform)
            .WithMany(gp => gp.GameTransactions)
            .HasForeignKey(gt => gt.GamePlatformId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
