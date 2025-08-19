using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class MessageEditSnapshotConfig : IEntityTypeConfiguration<MessageEditSnapshot>
{
    public void Configure(EntityTypeBuilder<MessageEditSnapshot> builder)
    {
        builder.ToTable("message_edit_snapshots");
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => new { x.MessageId, x.VersionNumber }).IsUnique();
        builder.HasIndex(x => x.EditorUserId);
        
        // Concurrency (Postgres xmin)
        builder.Property<uint>("xmin")
            .HasColumnName("xmin").HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
 
        builder.Property(x => x.MessageId)
            .HasColumnName("message_id")
            .IsRequired();
        
        builder.Property(x => x.VersionNumber)
            .HasColumnName("version_number")
            .IsRequired();
        
        builder.Property(x => x.EditorUserId)
            .HasColumnName("editor_user_id")
            .IsRequired();
        
        builder.Property(x => x.EditedAt)
            .HasColumnName("edited_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(x => x.MessageKind)
            .HasColumnName("message_kind")
            .IsRequired();
        
        builder.Property(x => x.Text)
            .HasColumnName("text")
            .IsRequired(false);
        
        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .IsRequired(false);
        
        builder.Property(x => x.ReplyToMessageId)
            .HasColumnName("reply_to_message_id")
            .IsRequired(false);
        
        builder.Property(x => x.IsTombstone)
            .HasColumnName("is_tombstone")
            .IsRequired();
        
        builder.Property(x => x.AttachmentIds)
            .HasColumnName("attachment_ids")
            .IsRequired(false);
        
        // Relationships
        builder.HasOne(x => x.Message)
            .WithMany(m => m.EditHistory)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.EditorUser)
            .WithMany()
            .HasForeignKey(x => x.EditorUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.ReplyToMessage)
            .WithMany()
            .HasForeignKey(x => x.ReplyToMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}