using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamDonationConfig : IEntityTypeConfiguration<LiveStreamDonation>
{
    public void Configure(EntityTypeBuilder<LiveStreamDonation> builder)
    {
        builder.ToTable("livestream_donations");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(d => d.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(d => d.PublicId)
            .IsUnique();

        builder.Property(d => d.LivestreamScheduleId)
            .IsRequired();

        builder.Property(d => d.DonorId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.GiftId)
            .IsRequired(false);

        builder.Property(d => d.Message)
            .HasMaxLength(2000);

        builder.Property(d => d.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.DonatedAt)
            .IsRequired();

        builder.HasOne(d => d.LivestreamSchedule)
            .WithMany()
            .HasForeignKey(d => d.LivestreamScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Donor)
            .WithMany()
            .HasForeignKey(d => d.DonorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Gift)
            .WithMany()
            .HasForeignKey(d => d.GiftId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
