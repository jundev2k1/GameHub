using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Events.Chat.OnDeleteMessage;

public record OnDeleteMessageEvent(DeletedMessageDto Dto) : IApplicationEvent;