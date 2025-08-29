using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class TransactionExternalConfig : IEntityTypeConfiguration<TransactionExternal>
{
    public void Configure(EntityTypeBuilder<TransactionExternal> builder)
    {
        builder.ToTable("transactions_external");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.G598Sno)
            .HasColumnName("g598_sno")
            .IsRequired(false);
        
        builder.Property(x => x.GamePlatformId)
            .HasColumnName("game_platform_id")
            .IsRequired();
    }
}