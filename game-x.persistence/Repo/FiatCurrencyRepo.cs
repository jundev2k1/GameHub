using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class FiatCurrencyRepo(GameXContext context) : IFiatCurrencyRepo, IRepository
{
    public async Task<FiatCurrency[]> GetAllAsync(CancellationToken ct = default)
    {
        var result = await context.FiatCurrencies
            .AsNoTracking()
            .Where(fc => fc.IsActive)
            .ToArrayAsync(ct);
        return result;
    }

    public async Task<FiatCurrency> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await context.FiatCurrencies
            .AsNoTracking()
            .FirstOrDefaultAsync(fc => fc.PublicId == id && fc.IsActive, ct)
            ?? throw new NotFoundException(nameof(FiatCurrency), id);
        return result;
    }
    public async Task<FiatCurrency> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var result = await context.FiatCurrencies
            .AsNoTracking()
            .FirstOrDefaultAsync(fc => fc.Id == id && fc.IsActive, ct)
            ?? throw new NotFoundException(nameof(FiatCurrency), id);
        return result;
    }

    public async Task<FiatCurrency> GetByCodeAsync(CurrencyUnit code, CancellationToken ct = default)
    {
        var result = await context.FiatCurrencies
            .AsNoTracking()
            .FirstOrDefaultAsync(fc => fc.Code.Equals(code) && fc.IsActive, ct)
            ?? throw new NotFoundException(nameof(FiatCurrency), code.Value);
        return result;
    }
}
