using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Events.Friendship.OnFriendUnblocked;

public record OnFriendUnblockedEvent(SocialLinkDto Dto) : IApplicationEvent;