using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Events.Friendship.OnRespondRequest;

public record OnRespondRequestEvent(SocialLinkDto Dto, ConversationSignalDto Conv) : IApplicationEvent;