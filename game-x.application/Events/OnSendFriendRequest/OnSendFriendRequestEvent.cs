using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Events.OnSendFriendRequest;

public record OnSendFriendRequestEvent(SocialLinkDto Dto) : IApplicationEvent;