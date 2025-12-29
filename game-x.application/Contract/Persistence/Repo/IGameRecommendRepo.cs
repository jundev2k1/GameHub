namespace game_x.application.Contract.Persistence.Repo;

public interface IGameRecommendRepo
{
    Task<GameRecommend[]> GetAllAsync(CancellationToken ct = default);

    Task<GameRecommend> GetAsync(Guid id, CancellationToken ct = default);

    Task<GameRecommend?> GetOverlapItemAsync(GameRecommend recommend, CancellationToken ct = default);

    Task AddAsync(GameRecommend recommend, CancellationToken ct = default);

    Task AddItemsAsync(Guid id, IEnumerable<GameRecommendItem> items, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Func<GameRecommend, Task> updateAction, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);

    Task DeleteAllItemsAsync(Guid id, CancellationToken ct = default);
}
