namespace game_x.application.Contract.Persistence.Repo;

public interface IUserBalanceRepo
{
    IQueryable<UserBalance> Query();

    Task<UserBalance?> GetByUserIdAndTokenIdAsync(string userId, int cryptoTokenId, CancellationToken ct = default);

    Task<(decimal totalUserAmount, decimal totalUserForzenAmount)> GetTotalUserAndAgentAvailableBalanceAsync(CancellationToken ct);

    Task PatchUpdateAsync(Guid publicId, Action<UserBalance> updateAction, CancellationToken ct = default);
    Task PutUpdateAsync(Guid publicId, UserBalance userBalance, CancellationToken ct = default);
}