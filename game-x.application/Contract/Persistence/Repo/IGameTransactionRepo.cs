namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTransactionRepo
{
    Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default);
}
