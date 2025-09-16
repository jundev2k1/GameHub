using System.Text.Json.Serialization;
using game_x.application.Common.Files;

namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed record SendSupportMessageCommand(
    [property: JsonIgnore] string SenderActorId, // guest or user
    [property: JsonIgnore] string? SenderUserId,
    string? Text,
    Guid? ReplyToMessageId,
    string ClientLocalId,
    IReadOnlyList<FileUpload>? Attachments
) : IRequest<SendSupportMessageResult>;

public record SendSupportMessageResult(string ClientLocalId);