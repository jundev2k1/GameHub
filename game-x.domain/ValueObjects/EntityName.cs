namespace game_x.domain.ValueObjects;

public sealed class EntityName
{
    public string Value { get; }

    private EntityName(string value) => Value = value;

    public static EntityName User => Of(nameof(Entities.User));
    public static EntityName MediaFile => Of(nameof(Entities.MediaFile));
    public static EntityName UserKyc => Of(nameof(Entities.UserKyc));
    public static EntityName Transaction => Of(nameof(Entities.Transaction));
    public static EntityName TransactionInternal => Of(nameof(Entities.TransactionInternal));
    public static EntityName TransactionExternal => Of(nameof(Entities.TransactionExternal));
    public static EntityName Game => Of(nameof(Entities.Game));
    public static EntityName GameType => Of(nameof(Entities.GameType));
    public static EntityName GameCategory => Of(nameof(Entities.GameCategory));
    public static EntityName GameTag => Of(nameof(Entities.GameTag));
    public static EntityName UserBalance => Of(nameof(Entities.UserBalance));
    public static EntityName FiatCurrency => Of(nameof(Entities.FiatCurrency));
    public static EntityName UserBankAccount => Of(nameof(Entities.UserBankAccount));
    public static EntityName Conversation => Of(nameof(Entities.Conversation));
    public static EntityName ConversationMember => Of(nameof(Entities.ConversationMember));
    public static EntityName Message => Of(nameof(Entities.Message));
    public static EntityName MessageAttachment => Of(nameof(Entities.MessageAttachment));
    public static EntityName LiveStreamCategory => Of(nameof(Entities.LiveStreamCategory));
    public static EntityName LivestreamSchedule => Of(nameof(Entities.LivestreamSchedule));
    public static EntityName SocialLink => Of(nameof(Entities.SocialLink));

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
        nameof(Entities.SocialLink),
    ];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}