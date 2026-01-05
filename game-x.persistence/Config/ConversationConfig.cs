using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class ConversationConfig : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations");
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => new { x.Type, x.LastMessageAt });
        builder.HasIndex(x => new { x.Status, x.LastMessageAt });
        builder.HasIndex(x => new { x.GuestId, x.LastMessageAt });
        builder.HasIndex(x => new { x.AssignedAgentId, x.Status });
        builder.HasIndex(x => new { CustomerUserId = x.CustomerId, x.Status });
        
        // Concurrency (Postgres xmin)
        builder.Property<uint>("xmin")
            .HasColumnName("xmin").HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(al => al.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
 
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasDefaultValue(ConversationType.Support);
        
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(ConversationStatus.Open);
        
        builder.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired(false);
        
        builder.Property(x => x.AssignedAgentId)
            .HasColumnName("assigned_agent_id")
            .IsRequired(false);

        builder.Property(x => x.LastMessageAt)
            .HasColumnName("last_message_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.LastResolvedAt)
            .HasColumnName("last_resolved_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.LastResolvedMessageId)
            .HasColumnName("last_resolved_message_id")
            .IsRequired(false);
        
        builder.Property(x => x.GuestId)
            .HasColumnName("guest_id")
            .IsRequired(false);
        
        // Relationships
        builder.HasMany(x => x.Members)
            .WithOne(m => m.Conversation)
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}