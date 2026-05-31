using System.Text.Json.Serialization;

namespace game_x.application.Features.Chat.Commands.MarkMessageAsRead;

public sealed record MarkMessageAsReadCommand(
    Guid ConversationId, 
    Guid LastReadMessageId,
    [property: JsonIgnore] string? GuestId = null) : ICommand;