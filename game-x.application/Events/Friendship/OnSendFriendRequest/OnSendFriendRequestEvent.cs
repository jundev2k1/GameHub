using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Events.Friendship.OnSendFriendRequest;

public record OnSendFriendRequestEvent(SocialLinkDto Dto) : IApplicationEvent;