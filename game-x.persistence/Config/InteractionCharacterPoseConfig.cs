using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class InteractionCharacterPoseConfig : IEntityTypeConfiguration<InteractionCharacterPose>
{
    public void Configure(EntityTypeBuilder<InteractionCharacterPose> builder)
    {
        builder.ToTable("interaction_character_poses");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(p => p.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(p => p.PublicId)
            .IsUnique();

        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Notes)
            .HasMaxLength(4000);

        builder.Property(p => p.Priority)
            .IsRequired();

        builder.Property(p => p.PoseId)
            .IsRequired();

        builder.HasOne(p => p.Pose)
            .WithMany()
            .HasForeignKey(p => p.PoseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
