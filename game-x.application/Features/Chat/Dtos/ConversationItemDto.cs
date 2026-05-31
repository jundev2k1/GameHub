namespace game_x.application.Features.Chat.Dtos;

public sealed record ConversationItemDto
{
    public Guid Id { get; init; }
    public string? GuestId { get; init; }
    public ConversationStatus Status { get; init; }
    public ConversationType Type { get; init; }
    public string? CustomerId { get; init; }
    public string? CustomerDisplayName { get; init; } 
    public MediaFile? CustomerAvatar { get; init; }
    public DateTime LastMessageAt { get; init; }
    public DateTime? LastResolvedAt { get; init; }
    public int? LastResolvedMessageId { get; init; }
    public DateTime? LastGuestReadAt { get; init; }
    public int? LastGuestReadMessageId { get; init; }
    public DateTime? LastUserReadAt { get; init; }
    public int? LastUserReadMessageId { get; init; }
    public LastMessageItemDto? LastMessage { get; init; }
    public bool? IsHidden { get; init; }
    public int? UnreadCount { get; init; }
    public string? CounterpartUserId { get; init; }
    public string? CounterpartDisplayName { get; init; } 
    public MediaFile? CounterpartAvatar { get; init; }
};

public sealed record LastMessageItemDto
{
    public int? Id { get; init; }
    public Guid? PublicId { get; init; }
    public string? SenderActorId { get; init; }
    public string? SenderName { get; init; } 
    public MediaFile? SenderAvatar { get; init; }
    public RoleInConversation? SenderRole { get; set; }
    public DateTime SentAt { get; init; } 
    public string? Text { get; init; } 
    public MessageKind Kind { get; init; }
}