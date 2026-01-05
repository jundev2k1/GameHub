using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Events.OnMarkMessageAsRead;

public record OnMarkMessageAsReadEvent(ConvUnreadDto Dto, string UserId, AppRole Role) : IApplicationEvent;