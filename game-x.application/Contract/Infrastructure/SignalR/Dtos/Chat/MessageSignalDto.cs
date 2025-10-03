using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MessageSignalDto(
    Guid Id,
    string ClientLocalId,
    Guid ConversationId,
    string SenderActorId,
    MessageKind Kind,
    RoleInConversation SenderRole,
    string? SenderUserNickname,
    string? SenderUserAvatarUrl,
    string? Text,
    Guid? ReplyToMessageId, 
    bool IsTombstone,
    bool? IsMentionAll,
    DateTime SentAt,
    DateTime? EditedAt, 
    int EditCount,
    int CurrentVersion,
    IReadOnlyList<MessageAttachmentSignalDto> Attachments,
    IReadOnlyList<DirectMention>? DirectMentions);

public record MessageAttachmentSignalDto(
    int SortOrder,
    string BindingStatus,
    string? FileName,
    string? Url);