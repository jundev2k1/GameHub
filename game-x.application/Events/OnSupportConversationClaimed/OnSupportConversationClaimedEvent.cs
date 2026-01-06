using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.OnSupportConversationClaimed;

public record OnSupportConversationClaimedEvent(ConversationSignalDto Dto) : IApplicationEvent;