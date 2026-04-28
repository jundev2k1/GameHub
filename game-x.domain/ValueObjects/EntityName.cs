using game_x.domain.Entities;

namespace game_x.domain.ValueObjects;

public sealed class EntityName
{
    public string Value { get; }

    private EntityName(string value) => Value = value;

    public static EntityName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("EntityName cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException("Entity name is not exists.", nameof(value));

        return new EntityName(value);
    }

    private static readonly string[] ValidValues = [
        nameof(AppSetting),
        nameof(User),
        nameof(MediaFile),
        nameof(UserKyc),
        nameof(SystemWallet),
        nameof(TalentWallet),
        nameof(Transaction),
        nameof(Game),
        nameof(GameTranslation),
        nameof(GameType),
        nameof(GameCategory),
        nameof(GameTag),
        nameof(GameRecommend),
        nameof(GameRecommendItem),
        nameof(UserBalance),
        nameof(FiatCurrency),
        nameof(UserBankAccount),
        nameof(Conversation),
        nameof(ConversationMember),
        nameof(Message),
        nameof(MessageAttachment),
        nameof(LivestreamSchedule),
        nameof(LiveStreamCategory),
        nameof(LiveStreamCategoryMapping),
        nameof(LiveStreamGift),
        nameof(LiveStreamGiftPrice),
        nameof(SocialLink),
        nameof(InteractionCharacter),
        nameof(InteractionCharacterPose),
        nameof(InteractionRule),
        nameof(InteractionRuleMessage),
    ];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
