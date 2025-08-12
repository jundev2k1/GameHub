namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTransactionRepo
{
    Task<bool> SnoExistsAsync(string sno, CancellationToken ct = default);
    Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default);
    Task<GameTransaction> UpdateAsync(GameTransaction entity, CancellationToken ct = default);
}