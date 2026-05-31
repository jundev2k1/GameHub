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

        var externalBalanceDtos = externalBalances.Adapt<UserWalletExternalItemDto[]>();
        var totalBalance = userDetail.Balances.Sum(b => b.TotalAmount);
        var totalGamePoint = externalBalanceDtos.Sum(b => b.TotalAmount);
        return userDetail.Adapt<GetUserDetailByAdminResult>() with
        {
            ExternalBalances = externalBalanceDtos,
            TotalBalance = totalBalance,
            TotalGamePoint = totalGamePoint,
            TotalAsset = totalBalance + totalGamePoint,
        };
    }
}