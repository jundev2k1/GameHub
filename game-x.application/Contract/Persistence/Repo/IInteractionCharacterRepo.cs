using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IInteractionCharacterRepo
{
    Task<PaginationResult<InteractionCharacter>> GetsByCriteriaAsync(
        Func<IQueryable<InteractionCharacter>, IQueryable<InteractionCharacter>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<InteractionCharacter> GetById(Guid id, CancellationToken ct = default);

    Task CreateAsync(InteractionCharacter character, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Action<InteractionCharacter> updateAction, CancellationToken ct = default);
    Task UpdateAsync(Guid id, Func<InteractionCharacter, Task> updateAction, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
