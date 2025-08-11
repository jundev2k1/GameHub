using Polly;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserBankAccountRepo
{
    Task<UserBankAccount[]> GetByUserIdAsync(string userId, CancellationToken ct = default);

    Task<UserBankAccount> GetByCodeAsync(Guid id, CancellationToken ct = default);

    Task Update(string userId, CurrencyUnit currencyCode, Action<UserBankAccount> updateAction, CancellationToken ct = default);
}
