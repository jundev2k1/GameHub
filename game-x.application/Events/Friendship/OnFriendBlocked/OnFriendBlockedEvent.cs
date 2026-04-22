using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Events.OnFriendBlocked;

public record OnFriendBlockedEvent(SocialLinkDto Dto) : IApplicationEvent;