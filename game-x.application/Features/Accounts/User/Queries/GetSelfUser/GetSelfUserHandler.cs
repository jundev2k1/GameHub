using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public sealed class GetSelfUserHandler(
    IUserAccessor userAccessor,
    IUserRepo appUserRepo,
    IWalletManagerCacheService walletManagerCache)
    : IQueryHandler<GetSelfUserQuery, GetSelfUserResult>
{
    public async Task<GetSelfUserResult> Handle(GetSelfUserQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var userDetail = await appUserRepo.GetUserDetailAsync(userId, ct);

        var balance = await walletManagerCache.GetWalletAsync(userId);
        var result = userDetail.Adapt<GetSelfUserResult>() with
        {
            InternalWallets = [.. balance.InternalWallets
                .Select(w => w.Adapt<GetSelfUserInternalInfo>())],
            ExternalWallets = [.. balance.ExternalWallets
                .Select(w => w.Adapt<GetSelfUserExternalInfo>())],
        };
        return result;
    }
}
