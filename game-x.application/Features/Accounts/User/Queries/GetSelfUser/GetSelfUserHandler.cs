using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public sealed class GetSelfUserHandler(
    IUserAccessor userAccessor,
    IUserRepo appUserRepo,
    IGameProviderService gameProviderService) : IQueryHandler<GetSelfUserQuery, GetSelfUserResult>
{
    public async Task<GetSelfUserResult> Handle(GetSelfUserQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var userDetail = await appUserRepo.GetUserDetailAsync(userId, ct);

        var externalBalance = await GetExternalWallet(userDetail.UserExtendInfo.GameProviderAccount);
        var result = userDetail.Adapt<GetSelfUserResult>() with
        {
            GameBalance = externalBalance
        };
        return result;
    }

    private async Task<decimal?> GetExternalWallet(string account)
    {
        try
        {
            var externalRequest = new WalletRequest { Account = account };
            var externalWallet = await gameProviderService.GetWalletAsync(externalRequest);
            return externalWallet.Quota;
        }
        catch
        {
            return null;
        }
    }
}
