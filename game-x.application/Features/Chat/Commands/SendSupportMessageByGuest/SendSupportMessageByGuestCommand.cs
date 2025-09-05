using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Chat.Commands.SendSupportMessageByGuest;

public sealed record SendSupportMessageByGuestCommand(
    [property: JsonIgnore] string GuestId,
    string Text
) : IRequest<SendSupportMessageByGuestResult>;

public sealed record SendSupportMessageByGuestResult(
    MessageDto Message,
    ConversationSignalDto Conv
);