namespace game_x.domain.Entities;

public sealed class InteractionRule : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public InteractionEventType EventType { get; private set; }
    public string ConditionExpression { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public RuleRepeatPolicy RepeatPolicy { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<InteractionRuleMessage> Messages { get; private set; } = [];

    public static InteractionRule Create(
        InteractionEventType eventType,
        int priority,
        RuleRepeatPolicy repeatPolicy,
        string conditionExpression = "{}")
    {
        return new InteractionRule
        {
            EventType = eventType,
            Priority = priority,
            RepeatPolicy = repeatPolicy,
            ConditionExpression = conditionExpression
        };
    }

    public void Update(
        InteractionEventType? eventType = null,
        int? priority = null,
        RuleRepeatPolicy? repeatPolicy = null,
        string? conditionExpression = null,
        bool? isActive = null)
    {
        if (eventType.HasValue)
            EventType = eventType.Value;
        if (priority.HasValue)
            Priority = priority.Value;
        if (repeatPolicy.HasValue)
            RepeatPolicy = repeatPolicy.Value;
        if (conditionExpression is not null)
            ConditionExpression = conditionExpression;
        if (isActive.HasValue)
            IsActive = isActive.Value;
    }
}
