namespace game_x.application.Contract.Persistence.Repo;

public interface IUserBalanceRepo
{
    Task<IEnumerable<UserBalance>> GetBalancesByUserIdAsync(string userId, CancellationToken ct = default);

    Task<UserBalance?> GetByUserIdAndTokenIdAsync(string userId, int cryptoTokenId, CancellationToken ct = default);
    Task<UserBalance> GetByUserIdAndTokenIdAsync(string userId, Guid cryptoTokenId, CancellationToken ct = default);

    Task BulkInsertAsync(IEnumerable<UserBalance> userBalances);

    Task UpdateAsync(int id, Action<UserBalance> updateAction, CancellationToken ct = default);
    Task UpdateAsync(Guid id, Action<UserBalance> updateAction, CancellationToken ct = default);
    Task UpdateAsync(Guid id, Func<UserBalance, Task> updateAction, CancellationToken ct = default);

    Task UpdateByTokenIdAsync(int id, Action<UserBalance> updateAction, CancellationToken ct = default);

    Task PutUpdateAsync(UserBalance userBalance, CancellationToken ct = default);
}
