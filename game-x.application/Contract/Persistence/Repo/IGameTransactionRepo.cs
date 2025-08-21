namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTransactionRepo
{
    Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default);
    /// <summary>Only update the fields that are passed in.</summary>
    Task PatchUpdateAsync(Guid publicId, Action<GameTransaction> updateAction, CancellationToken ct = default);
    /// <summary>Override all data of the record.</summary>
    Task PutUpdateAsync(GameTransaction transaction, CancellationToken ct = default);
}
