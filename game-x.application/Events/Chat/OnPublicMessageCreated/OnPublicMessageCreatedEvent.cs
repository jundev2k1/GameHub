using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.Chat.OnPublicMessageCreated;

public record OnPublicMessageCreatedEvent(CreatedMessageSignalResult Res) : IApplicationEvent;