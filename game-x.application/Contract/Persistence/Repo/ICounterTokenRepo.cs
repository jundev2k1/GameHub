namespace game_x.application.Contract.Persistence.Repo;

public interface ICounterTokenRepo
{
    Task AddAsync(CounterToken counterToken, CancellationToken ct = default);

    Task<CounterToken> GetByTokenAsync(string token, CancellationToken ct = default);

    Task<CounterToken> GetByIdAsync(Guid counterCode, CancellationToken ct = default);
}