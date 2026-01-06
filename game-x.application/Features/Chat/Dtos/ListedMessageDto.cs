namespace game_x.application.Features.Chat.Dtos;

public record ListedMessageDto(
    Guid Id,
    int Index,
    Guid ConversationId,
    string SenderActorId,
    MessageKind Kind,
    RoleInConversation SenderRole,
    string? SenderUserNickname,
    string? SenderUserAvatarUrl,
    string? Text,
    Guid? ReplyToMessageId, 
    bool IsTombstone, 
    DateTime SentAt, 
    DateTime? EditedAt, 
    int EditCount, 
    int CurrentVersion,
    bool? IsMentionAll,
    IReadOnlyList<DirectMention>? DirectMentions,
    IReadOnlyList<ListedMessageAttachmentDto> Attachments);
    
public record ListedMessageAttachmentDto(
    int SortOrder,
    string BindingStatus,
    string? FileName,
    string? Url);