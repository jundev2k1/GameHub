using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.Chat.OnMarkMessageAsRead;

public record OnMarkMessageAsReadEvent(ConversationSignalDto Dto, string UserId, AppRole Role) : IApplicationEvent;