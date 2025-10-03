using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class InteractionRuleConfig : IEntityTypeConfiguration<InteractionRule>
{
    public void Configure(EntityTypeBuilder<InteractionRule> builder)
    {
        builder.ToTable("interaction_rules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(r => r.PublicId)
            .IsRequired()
            .HasColumnName("code")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(r => r.PublicId)
            .IsUnique();

        builder.Property(r => r.EventType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(r => r.ConditionExpression)
            .HasMaxLength(4000);

        builder.Property(r => r.Priority)
            .IsRequired();

        builder.Property(r => r.RepeatPolicy)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(r => r.Messages)
            .WithOne(m => m.Rule)
            .HasForeignKey(m => m.RuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
