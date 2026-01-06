using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class S2sClientRepo(GameXContext dbContext) : IS2sClientRepo, IRepository
{
    public async Task<S2SClient[]> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.S2sClients
            .AsNoTracking()
            .Include(sc => sc.Settings)
            .ToArrayAsync(ct);
    }

    public async Task<bool> IsExistAsync(string clientId, CancellationToken ct = default)
    {
        return await dbContext.S2sClients.AsNoTracking().AnyAsync(s => s.Id == clientId, ct);
    }

    public async Task CreateAsync(S2SClient entity, CancellationToken ct = default)
    {
        await dbContext.S2sClients.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(string clientId, Action<S2SClient> updateAction, CancellationToken ct = default)
    {
        var target = await dbContext.S2sClients
            .FirstOrDefaultAsync(sc => sc.Id == clientId, ct)
            ?? throw new NotFoundException(nameof(clientId), clientId);

        updateAction?.Invoke(target);
    }

    public async Task DeleteAsync(string clientId, CancellationToken ct = default)
    {
        var target = await dbContext.S2sClients
            .FirstOrDefaultAsync(sc => sc.Id == clientId, ct)
            ?? throw new NotFoundException(nameof(clientId), clientId);

        dbContext.S2sClients.Remove(target);
    }
}
