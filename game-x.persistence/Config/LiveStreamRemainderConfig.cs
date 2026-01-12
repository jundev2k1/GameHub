using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamRemainderConfig : IEntityTypeConfiguration<LiveStreamReminder>
{
    public void Configure(EntityTypeBuilder<LiveStreamReminder> builder)
    {
        builder.ToTable("livestream_reminders");

        builder.HasKey(lr => lr.Id);

        builder.Property(lr => lr.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(lr => lr.ScheduleId)
            .IsRequired();

        builder.Property(lr => lr.UserId)
            .IsRequired();

        builder.Property(lr => lr.Status)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(lr => lr.SentAt)
            .IsRequired(false);

        builder.Property(lr => lr.Channel)
            .IsRequired()
            .HasConversion<short>();

        builder.HasOne(lr => lr.Schedule)
            .WithMany()
            .HasForeignKey(lr => lr.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lr => lr.User)
            .WithMany()
            .HasForeignKey(lr => lr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(lr => new { lr.ScheduleId, lr.UserId, lr.Channel }).IsUnique();
    }
}
