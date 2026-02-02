using System.Text.Json.Serialization;
using game_x.application.Common.Files;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.SendMessage;

public sealed record SendMessageCommand(
    Guid ConversationId,
    string ClientLocalId,
    MessageKind Kind,
    [property: JsonIgnore] string SenderActorId, // guest or user
    [property: JsonIgnore] string? SenderUserId = null,
    string? Text = null,
    Guid? ReplyToMessageId = null,
    [property: JsonIgnore] bool? IsAgent = null,
    IReadOnlyList<FileUpload>? Attachments = null,
    MentionRequest? Mention = null
) : IRequest<SendMessageResult>;

public sealed record SendMessageResult(string ClientLocalId, Guid ConversationId);
public sealed record MentionRequest(
    bool IsAll,
    IReadOnlyList<DirectMention>? Direct // userIds to mention
);