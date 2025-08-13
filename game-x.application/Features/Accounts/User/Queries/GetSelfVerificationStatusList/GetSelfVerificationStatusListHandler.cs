using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfVerificationStatusList;

public sealed class GetSelfVerificationStatusListHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo) : IQueryHandler<GetSelfVerificationStatusListQuery, VerificationStatusDto[]>
{
    public async Task<VerificationStatusDto[]> Handle(
        GetSelfVerificationStatusListQuery request,
        CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        return await userRepo.GetVerificationStatusList(userId, ct);
    }
}
