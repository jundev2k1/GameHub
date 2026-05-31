using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.Chat.OnSupportMessageCreatedV2;

public record OnSupportMessageCreatedV2Event(CreatedMessageSignalResult Res) : IApplicationEvent;