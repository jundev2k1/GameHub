using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.TalentWallets.Queries.GetTalentWallet;

public sealed class GetTalentWalletHandler(
    IUserAccessor userAccessor,
    ITalentWalletRepo talentWalletRepo) : IQueryHandler<GetTalentWalletQuery, GetTalentWalletResult>
{
    public async Task<GetTalentWalletResult> Handle(GetTalentWalletQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var wallet = await talentWalletRepo.GetWalletAsync(userId, ct);
        return new GetTalentWalletResult(wallet.Balance);
    }
}
