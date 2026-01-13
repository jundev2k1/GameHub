using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;

public sealed class GetUserDetailByAdminHandler(
    IUserRepo userRepo,
    IGamePlatformBalanceRepo platformBalanceRepo) : IQueryHandler<GetUserDetailByAdminQuery, GetUserDetailByAdminResult>
{
    public async Task<GetUserDetailByAdminResult> Handle(GetUserDetailByAdminQuery request, CancellationToken ct = default)
    {
        var userDetail = await userRepo.GetUserDetailAsync(request.UserId, ct);
        var externalBalances = await platformBalanceRepo.GetBalancesByUserIdAsync(userDetail.UserId, ct);
        return userDetail.Adapt<GetUserDetailByAdminResult>() with
        {
            Roles = userDetail.Roles.Items,
            InternalBalances = userDetail.Balances,
            ExternalBalances = [..externalBalances.Select(eb => new UserWalletExternalItemDto
            {
                PlatformId = eb.Platform.PublicId,
                PlatformName = eb.Platform.Name,
                Amount = eb.AvailableBalance,
                LockedAmount = eb.LockedBalance,
                LastSyncAt = eb.LastSyncedAt,
            })],
        };
    }
}
