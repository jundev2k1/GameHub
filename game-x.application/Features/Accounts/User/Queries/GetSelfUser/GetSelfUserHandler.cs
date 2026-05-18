using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public sealed class GetSelfUserHandler(
    IUserAccessor userAccessor,
    IUserRepo appUserRepo,
    IWalletManagerCacheService walletManagerCache,
    IUserInventoryCacheService userInventoryCache,
    ILogger<GetSelfUserHandler> logger)
    : IQueryHandler<GetSelfUserQuery, GetSelfUserResult>
{
    public async Task<GetSelfUserResult> Handle(GetSelfUserQuery request, CancellationToken ct = default)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var userDetail = await appUserRepo.GetUserDetailAsync(userId, ct);

            var inventories = await userInventoryCache.GetAll(userDetail.UserId, ct);
            
            var balance = await walletManagerCache.GetWalletAsync(userId);
            var result = userDetail.Adapt<GetSelfUserResult>() with
            {
                InternalWallets = [.. balance.InternalWallets
                    .Select(w => w.Adapt<GetSelfUserInternalInfo>())],
                ExternalWallets = [.. balance.ExternalWallets
                    .Select(w => w.Adapt<GetSelfUserExternalInfo>())],
                Inventories = inventories ?? []
            };
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}