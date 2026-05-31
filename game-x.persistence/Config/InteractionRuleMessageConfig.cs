using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class InteractionRuleMessageConfig : IEntityTypeConfiguration<InteractionRuleMessage>
{
    public void Configure(EntityTypeBuilder<InteractionRuleMessage> builder)
    {
        builder.ToTable("interaction_rule_messages");

        builder.HasKey(m => new { m.RuleId, m.LanguageCode });

        builder.Property(m => m.LanguageCode)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => LanguageCode.Of(v))
            .HasMaxLength(10);

        builder.Property(m => m.Text)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Ignore(m => m.Id);

        builder.HasOne(m => m.Character)
            .WithMany()
            .HasForeignKey(m => m.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Pose)
            .WithMany()
            .HasForeignKey(m => m.PoseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.VoiceMedia)
            .WithMany()
            .HasForeignKey(m => m.VoiceMediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

