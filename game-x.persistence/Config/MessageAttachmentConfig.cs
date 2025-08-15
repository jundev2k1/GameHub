using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class MessageAttachmentConfig : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("message_attachments");
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => new { x.MessageId, x.SortOrder }).IsUnique();
        builder.HasIndex(x => new { x.MessageId, x.MediaFileId })
            .IsUnique()
            .HasFilter("\"media_file_id\" IS NOT NULL");
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
 
        builder.Property(x => x.MessageId)
            .HasColumnName("message_id")
            .IsRequired();
        
        builder.Property(x => x.MediaFileId)
            .HasColumnName("media_file_id")
            .IsRequired(false);
        
        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();
        
        builder.Property(x => x.AddedByUserId)
            .HasColumnName("added_by_user_id")
            .IsRequired();
        
        // Relationships
        builder.HasOne(x => x.MediaFile)
            .WithMany()
            .HasForeignKey(x => x.MediaFileId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}