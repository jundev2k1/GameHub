using game_x.domain.Entities;
using game_x.domain.Entities.Rewards;

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
        nameof(GamePlatform),
        nameof(GamePlatformTranslation),
        nameof(Game),
        nameof(GameTranslation),
        nameof(GameType),
        nameof(GameTypeTranslation),
        nameof(GameCategory),
        nameof(GameCategoryTranslation),
        nameof(GameTag),
        nameof(GameTagTranslation),
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
        nameof(CatalogItem),
        nameof(Mission),
        nameof(RewardPoolItem),
        nameof(RewardPool),
        nameof(UserReward),
        nameof(RewardDefinition),
        nameof(MissionReward),
        nameof(UserMissionClaim)
    ];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
