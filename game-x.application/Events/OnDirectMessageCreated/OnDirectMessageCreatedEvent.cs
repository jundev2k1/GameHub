using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.OnDirectMessageCreated;

public record OnDirectMessageCreatedEvent(CreatedMessageSignalResult Res) : IApplicationEvent;