using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.OnSupportMessageCreated;

public record OnSupportMessageCreatedEvent(CreatedMessageSignalResult Res) : IApplicationEvent;