using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class TransactionInternalConfig : IEntityTypeConfiguration<TransactionInternal>
{
    public void Configure(EntityTypeBuilder<TransactionInternal> builder)
    {
        builder.ToTable("transactions_internal");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.OrderUid)
            .HasColumnName("order_uid")
            .IsRequired(false);
        
        builder.Property(x => x.OrderNumber)
            .HasColumnName("order_number")
            .IsRequired(false);
        
        builder.Property(x => x.Hash)
            .HasColumnName("hash")
            .IsRequired(false);
        
        builder.Property(x => x.FromAddress)
            .HasColumnName("from_address")
            .IsRequired(false);
        
        builder.Property(x => x.ToAddress)
            .HasColumnName("to_address")
            .IsRequired(false);
        
        builder.Property(x => x.ConfirmedAt)
            .HasColumnName("confirmed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        
        builder.Property(x => x.ProviderId)
            .HasColumnName("provider_id")
            .IsRequired(false);
        
        builder.Property(x => x.SourceType)
            .HasColumnName("source_type")
            .IsRequired();
        
        builder.Property(x => x.ReferenceId)
            .HasColumnName("reference_id")
            .IsRequired(false);
        
        builder.HasOne(i => i.Transaction)
            .WithOne(t => t.TransactionInternal)
            .HasForeignKey<TransactionInternal>(i => i.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}