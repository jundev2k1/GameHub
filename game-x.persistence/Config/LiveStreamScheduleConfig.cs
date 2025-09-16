using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamScheduleConfig : IEntityTypeConfiguration<LivestreamSchedule>
{
    public void Configure(EntityTypeBuilder<LivestreamSchedule> builder)
    {
        builder.ToTable("livestream_schedules");

        builder.HasKey(ls => ls.Id);

        builder.Property(ls => ls.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(ls => ls.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(ls => ls.PublicId)
            .IsUnique();

        builder.Property(ls => ls.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ls => ls.Description)
            .HasMaxLength(4000);

        builder.Property(ls => ls.Notes)
            .HasMaxLength(4000);

        builder.Property(ls => ls.StartTime)
            .IsRequired();

        builder.Property(ls => ls.EndTime)
            .IsRequired();

        builder.Property(ls => ls.StartAt)
            .IsRequired(false);

        builder.Property(ls => ls.EndAt)
            .IsRequired(false);

        builder.Property(ls => ls.StreamKey)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ls => ls.Status)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(ls => ls.CancellationReason)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(ls => ls.AssignedId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.HasOne(ls => ls.AssignedBy)
            .WithMany()
            .HasForeignKey(ls => ls.AssignedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
