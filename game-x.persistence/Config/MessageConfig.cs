using game_x.share.Helper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace game_x.persistence.Config;

public sealed class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");
        builder.HasKey(x => x.Id);
        
        // Indexes
        builder.HasIndex(x => new { x.ConversationId, x.SentAt });
        builder.HasIndex(x => x.ReplyToMessageId);
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(al => al.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(x => x.ConversationId)
            .HasColumnName("conversation_id")
            .IsRequired();
        
        builder.Property(x => x.SenderActorId)
            .HasColumnName("sender_actor_id")
            .IsRequired();
        
        builder.Property(x => x.SenderUserId)
            .HasColumnName("sender_user_id")
            .IsRequired(false);
        
        builder.Property(x => x.SenderRole)
            .HasColumnName("sender_role")
            .IsRequired();
        
        builder.Property(x => x.Kind)
            .HasColumnName("kind")
            .IsRequired()
            .HasDefaultValue(MessageKind.Text);

        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(x => x.ReplyToMessageId)
            .HasColumnName("reply_to_message_id")
            .IsRequired(false);
        
        builder.Property(x => x.IsTombstone)
            .HasColumnName("is_tombstone")
            .IsRequired();
        
        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(x => x.EditedAt)
            .HasColumnName("edited_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        
        builder.Property(x => x.EditCount)
            .HasColumnName("edit_count")
            .IsRequired();
        
        builder.Property(x => x.CurrentVersion)
            .HasColumnName("current_version")
            .IsRequired();
        
        builder.Property(x => x.Reactions)
            .HasColumnName("reactions")
            .IsRequired();
        
        // Map JSONB
        var converter = new ValueConverter<Dictionary<string, HashSet<string>>, string>(
            v => DictSetJsonbHelper.ToJson(v),
            v => DictSetJsonbHelper.FromJson(v)
        );

        var comparer = new ValueComparer<Dictionary<string, HashSet<string>>>(
            (a, b) => DictSetJsonbHelper.Equals(a, b),
            v => DictSetJsonbHelper.GetHashCode(v),
            v => DictSetJsonbHelper.Snapshot(v)
        );
        
        builder.Property(x => x.Reactions)
            .HasColumnName("reactions")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .HasConversion(converter)
            .IsRequired()
            .Metadata.SetValueComparer(comparer);
    }
}