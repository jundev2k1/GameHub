namespace game_x.application.Contract.Persistence.Repo;

public interface IFiatCurrencyRepo
{
    Task<FiatCurrency[]> GetAllAsync(CancellationToken ct = default);

    Task<FiatCurrency> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<FiatCurrency> GetByIdAsync(int id, CancellationToken ct = default);
}
