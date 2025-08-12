using Polly;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserBankAccountRepo
{
    Task<UserBankAccount[]> GetByUserIdAsync(string userId, CancellationToken ct = default);

    Task<UserBankAccount> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<UserBankAccount> GetByCurencyCodeAsync(string userId, CurrencyUnit curencyCode, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Action<UserBankAccount> updateAction, CancellationToken ct = default);
    Task UpdateAsync(string userId, CurrencyUnit currencyCode, Action<UserBankAccount> updateAction, CancellationToken ct = default);
}
