using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;

public sealed class GetSelfUserBalanceHandler(IUserBalanceRepo userBalanceRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetSelfUserBalanceQuery, IEnumerable<GetSelfUserBalanceResult>>
{
    public async Task< IEnumerable<GetSelfUserBalanceResult>> Handle(GetSelfUserBalanceQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var balances = await userBalanceRepo.GetBalancesByUserIdAsync(userId, ct);
        return balances.Select(item => item.Adapt<GetSelfUserBalanceResult>());
    }
}
