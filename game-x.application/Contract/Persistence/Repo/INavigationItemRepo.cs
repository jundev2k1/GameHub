namespace game_x.application.Contract.Persistence.Repo;

public interface INavigationItemRepo
{
    Task<NavigationItem[]> GetAllAsync(CancellationToken ct = default);

    Task<NavigationItem> GetAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(NavigationItem item, CancellationToken ct = default);

    Task UpdateAsync(
        Guid id,
        Action<NavigationItem> updateAction,
        Func<IQueryable<NavigationItem>, IQueryable<NavigationItem>>? preUpdateAction = null,
        CancellationToken ct = default);

    Task UpdateTranslationAsync(
        Guid id,
        Action<NavigationItem> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
