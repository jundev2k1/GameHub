using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.CheckFriendSocialLink;

public record CheckFriendSocialLinkQuery(string UserId) : IQuery<SocialLinkDto>;