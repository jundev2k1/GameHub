using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class SystemWalletRepo(GameXContext dbContext) : ISystemWalletRepo, IRepository
{
    public async Task UpdateAsync(SystemWalletType type, Action<SystemWallet> updateAction, CancellationToken ct = default)
    {
        var targetWallet = await dbContext.SystemWallets.FirstOrDefaultAsync(sw => sw.Type == type, ct)
            ?? throw new NotFoundException(nameof(type), type.ToString());

        updateAction.Invoke(targetWallet);
    }
}
