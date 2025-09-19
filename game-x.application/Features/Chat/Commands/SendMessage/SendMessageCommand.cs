using System.Text.Json.Serialization;
using game_x.application.Common.Files;

namespace game_x.application.Features.Chat.Commands.SendMessage;

public sealed record SendMessageCommand(
    [property: JsonIgnore] string SenderActorId, // guest or user
    [property: JsonIgnore] string? SenderUserId,
    Guid ConversationId,
    string? Text,
    Guid? ReplyToMessageId,
    string ClientLocalId,
    IReadOnlyList<FileUpload>? Attachments
) : IRequest<SendMessageResult>;

public record SendMessageResult(string ClientLocalId, Guid ConversationId);