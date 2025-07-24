using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;
public sealed class CounterTokenRepo(GameXContext context) : ICounterTokenRepo
{
    public async Task AddAsync(CounterToken counterToken, CancellationToken ct = default)
    {
        await context.CounterTokens.AddAsync(counterToken, ct);
    }

    public async Task<CounterToken> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await context.CounterTokens
            .Include(c => c.Counter)
            .FirstOrDefaultAsync(c => (c.Token == token) && c.IsValid, ct)
            ?? throw new NotFoundException(nameof(token), token);
    }

    public async Task<CounterToken> GetByIdAsync(Guid counterCode, CancellationToken ct = default)
    {
        return await context.CounterTokens.FirstOrDefaultAsync(
            ct => (ct.Counter.PublicId == counterCode) && ct.IsValid, ct)
            ?? throw new NotFoundException(nameof(counterCode), counterCode);
    }
}
