namespace game_x.application.Contract.Persistence.Repo;

public interface IUserUsdtLedgerRepo
{
    IQueryable<UserUsdtLedger> Query();
    Task<UserUsdtLedger?> GetLatestLedgerAsync(string userId);
    Task AddAsync(UserUsdtLedger userUsdtLedger, CancellationToken ct = default);
}