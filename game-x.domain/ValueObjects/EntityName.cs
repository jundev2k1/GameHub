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
        nameof(Entities.User),
        nameof(Entities.MediaFile),
        nameof(Entities.UserKyc),
        nameof(Entities.Transaction),
        nameof(Entities.TransactionInternal),
        nameof(Entities.TransactionExternal),
        nameof(Entities.Game),
        nameof(Entities.GameType),
        nameof(Entities.GameCategory),
        nameof(Entities.GameTag),
        nameof(Entities.GameRecommend),
        nameof(Entities.GameRecommendItem),
        nameof(Entities.UserBalance),
        nameof(Entities.FiatCurrency),
        nameof(Entities.UserBankAccount),
        nameof(Entities.Conversation),
        nameof(Entities.ConversationMember),
        nameof(Entities.Message),
        nameof(Entities.MessageAttachment),
        nameof(Entities.LivestreamSchedule),
        nameof(Entities.LiveStreamCategory),
        nameof(Entities.LiveStreamCategoryMapping),
        nameof(Entities.LiveStreamGift),
        nameof(Entities.LiveStreamGiftPrice),
        nameof(Entities.SocialLink),
        nameof(Entities.InteractionCharacter),
        nameof(Entities.InteractionCharacterPose),
    ];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
