using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserUsdtLedgerConfig: IEntityTypeConfiguration<UserUsdtLedger>
{
    public void Configure(EntityTypeBuilder<UserUsdtLedger> builder)
    {
        builder.ToTable("user_usdt_ledgers");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PublicId).IsUnique();
        
        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);
        
        builder.Property(x => x.Timestamp)
            .HasColumnName("timestamp")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(x => x.SourceId)
            .HasColumnName("source_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);
        
        builder.Property(x => x.ChainTransactionId)
            .HasColumnName("chain_transaction_id")
            .IsRequired(false);
        
        builder.Property(x => x.Meta)
            .HasColumnName("meta")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");
        
        builder.HasOne(x => x.User)
            .WithMany(u => u.UserUsdtLedgers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}