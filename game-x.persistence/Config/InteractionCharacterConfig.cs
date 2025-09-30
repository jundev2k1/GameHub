using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class InteractionCharacterConfig : IEntityTypeConfiguration<InteractionCharacter>
{
    public void Configure(EntityTypeBuilder<InteractionCharacter> builder)
    {
        builder.ToTable("interaction_characters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(c => c.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(c => c.PublicId)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Notes)
            .HasMaxLength(4000);

        builder.Property(c => c.DefaultPoseId)
            .IsRequired();

        builder.HasOne(c => c.DefaultPose)
            .WithMany()
            .HasForeignKey(c => c.DefaultPoseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Poses)
            .WithOne(p => p.Character)
            .HasForeignKey(p => p.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
