using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.CheckFriendSocialLink;

public class CheckFriendSocialLinkHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo): IQueryHandler<CheckFriendSocialLinkQuery, SocialLinkDto>
{
    public async Task<SocialLinkDto> Handle(CheckFriendSocialLinkQuery request, CancellationToken ct)
    {
        string me = userAccessor.GetUserId();
        var (min, max) = SocialLinkPair.Normalize(me, request.UserId);
        var link = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        
        return link?.Adapt<SocialLinkDto>()!;
    }
}