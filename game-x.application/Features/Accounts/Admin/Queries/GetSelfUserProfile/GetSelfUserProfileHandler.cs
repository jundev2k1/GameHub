using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.Admin.Queries.GetSelfUserProfile;

public sealed class GetSelfUserProfileHandler(IUserAccessor userAccessor, IUserRepo userRepo)
    : IQueryHandler<GetSelfUserProfileQuery, GetSelfUserProfileResult>
{
    public async Task<GetSelfUserProfileResult> Handle(GetSelfUserProfileQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await userRepo.GetAdminById(userId, ct);
        return result.Adapt<GetSelfUserProfileResult>();
    }
}
