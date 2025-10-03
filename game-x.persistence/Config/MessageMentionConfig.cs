using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class MessageMentionConfig : IEntityTypeConfiguration<MessageMention>
{
    public void Configure(EntityTypeBuilder<MessageMention> builder)
    {
        builder.ToTable("message_mentions");
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(mm => new { mm.UserId, mm.MessageId }).IsUnique();
        builder.HasIndex(mm => mm.MessageId);
        builder.HasIndex(mm => mm.UserId);
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
 
        builder.Property(x => x.MessageId)
            .HasColumnName("message_id")
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(x => x.Kind)
            .HasColumnName("kind")
            .IsRequired();
        
        builder.Property(x => x.Display)
            .HasColumnName("display")
            .IsRequired(false);
        
        builder.Property(x => x.DeliveredAt)
            .HasColumnName("delivered_at")
            .IsRequired(false);
        
        builder.Property(x => x.ReadAt)
            .HasColumnName("read_at")
            .IsRequired(false);
        
        // Relationships
        builder.HasOne(x => x.Message)
            .WithMany(m => m.Mentions)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}