using System.Text.Json.Serialization;
using game_x.application.Common.Files;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.SendMessage;

public sealed record SendMessageCommand(
    [property: JsonIgnore] string SenderActorId, // guest or user
    [property: JsonIgnore] string? SenderUserId,
    [property: JsonIgnore] bool? IsAgent,
    Guid ConversationId,
    string? Text,
    Guid? ReplyToMessageId,
    string ClientLocalId,
    IReadOnlyList<FileUpload>? Attachments,
    MentionRequest? Mention
) : IRequest<SendMessageResult>;

public sealed record SendMessageResult(string ClientLocalId, Guid ConversationId);
public sealed record MentionRequest(
    bool IsAll,
    IReadOnlyList<DirectMention>? Direct // userIds to mention
);