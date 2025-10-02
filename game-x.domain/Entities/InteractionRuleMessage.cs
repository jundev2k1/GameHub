namespace game_x.domain.Entities;

public sealed class InteractionRuleMessage : BaseEntity<int>, IAuditable
{
    public int RuleId { get; private set; }
    public InteractionRule Rule { get; private set; } = default!;
    public LanguageCode LanguageCode { get; private set; } = LanguageCode.Of(LanguageCodeValue.Taiwanese);
    public string Text { get; private set; } = string.Empty;
    public int? VoiceMediaId { get; private set; }
    public MediaFile? VoiceMedia { get; private set; }
    public int CharacterId { get; private set; }
    public InteractionCharacter Character { get; private set; } = default!;
    public int? PoseId { get; private set; }
    public InteractionCharacterPose? Pose { get; private set; }

    public static InteractionRuleMessage Create(
        LanguageCode languageCode,
        string text,
        InteractionCharacter character,
        InteractionCharacterPose? pose = null,
        MediaFile? voiceMedia = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        return new InteractionRuleMessage
        {
            LanguageCode = languageCode,
            Text = text.Trim(),
            Character = character,
            Pose = pose,
            VoiceMedia = voiceMedia
        };
    }

    public MediaFile GetEffectivePose()
        => Pose?.Pose ?? Character.DefaultPose;
}
