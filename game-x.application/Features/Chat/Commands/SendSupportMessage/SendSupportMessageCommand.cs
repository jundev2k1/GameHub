using System.Text.Json.Serialization;
using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed record SendSupportMessageCommand(
    [property: JsonIgnore] string SenderActorId, // guest or user
    [property: JsonIgnore] string? SenderUserId,
    string? Text,
    Guid? ReplyToMessageId,
    IReadOnlyList<FileUpload>? Attachments
) : IRequest<Unit>;