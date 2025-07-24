namespace game_x.application.Contract.Persistence.Repo;

public interface IUserPassportRepo
{
    Task<UserPassport> GetByUserIdAsync(string userId, CancellationToken ct = default);

    Task<UserPassport> GetByPassportNumberAsync(string passportNumber, CancellationToken ct = default);

    Task<bool> IsExistsByPassportNumberAsync(string passportNumber, CancellationToken ct = default);

    Task AddAsync(UserPassport passport, CancellationToken ct = default);

    Task UpdatePassportAsync(string passportNumber, Action<UserPassport> updateAction, CancellationToken ct = default);
}
