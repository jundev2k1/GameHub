using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public sealed class GetSelfUserHandler(IUserRepo appUserRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetSelfUserQuery, GetSelfUserResult>
{
    public async Task<GetSelfUserResult> Handle(GetSelfUserQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var userDetail = await appUserRepo.GetUserDetailAsync(userId, ct);
        return userDetail.Adapt<GetSelfUserResult>();
    }
}
