using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamChatMessageConfig : IEntityTypeConfiguration<LiveStreamChatMessage>
{
    public void Configure(EntityTypeBuilder<LiveStreamChatMessage> builder)
    {
        builder.ToTable("livestream_chat_messages");

        builder.HasKey(lcm => lcm.Id);

        builder.Property(lcm => lcm.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(lcm => lcm.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(lcm => lcm.PublicId)
            .IsUnique();

        builder.Property(lcm => lcm.LiveStreamId)
            .IsRequired();

        builder.Property(lcm => lcm.SenderId)
            .IsRequired();

        builder.Property(lcm => lcm.Message)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(lcm => lcm.MessageType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(lcm => lcm.DonationAmount)
            .IsRequired(false);

        builder.Property(lcm => lcm.SentAt)
            .IsRequired();

        builder.Ignore(lcm => lcm.CreatedAt);
        builder.Ignore(lcm => lcm.UpdatedAt);

        builder.HasOne(lcm => lcm.Sender)
            .WithMany()
            .HasForeignKey(lcm => lcm.SenderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lcm => lcm.LiveStream)
            .WithMany()
            .HasForeignKey(lcm => lcm.LiveStreamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
