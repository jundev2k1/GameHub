using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class TalentWalletRepo(GameXContext dbContext) : ITalentWalletRepo, IRepository
{
    public async Task UpdateAsync(string userId, Action<TalentWallet> updateAction, CancellationToken ct = default)
    {
        var target = await dbContext.TalentWallets
            .FirstOrDefaultAsync(tw => tw.Id == userId, ct)
            ?? throw new NotFoundException(nameof(userId), userId);

        updateAction.Invoke(target);
    }
}
