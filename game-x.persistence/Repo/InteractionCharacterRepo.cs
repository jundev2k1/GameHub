using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class InteractionCharacterRepo(GameXContext context) : IInteractionCharacterRepo, IRepository
{
    public async Task<PaginationResult<InteractionCharacter>> GetsByCriteriaAsync(
        Func<IQueryable<InteractionCharacter>, IQueryable<InteractionCharacter>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.InteractionCharacters
            .AsQueryable();

        if (builder is not null)
            query = builder(query);

        var totalItems = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);
        return new PaginationResult<InteractionCharacter>(
            items,
            totalItems,
            totalPages,
            page,
            pageSize);
    }

    public async Task<InteractionCharacter> GetById(Guid id, CancellationToken ct = default)
    {
        return await context.InteractionCharacters
            .Include(ic => ic.DefaultPose)
            .Include(ic => ic.Poses)
            .ThenInclude(ic => ic.Pose)
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task CreateAsync(InteractionCharacter character, CancellationToken ct = default)
    {
        await context.InteractionCharacters.AddAsync(character, ct);
    }

    public async Task UpdateAsync(Guid id, Action<InteractionCharacter> updateAction, CancellationToken ct = default)
    {
        var character = await context.InteractionCharacters
            .Include(ic => ic.DefaultPose)
            .Include(ic => ic.Poses)
            .ThenInclude(p => p.Pose)
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        updateAction(character);
    }
    public async Task UpdateAsync(Guid id, Func<InteractionCharacter, Task> updateAction, CancellationToken ct = default)
    {
        var character = await context.InteractionCharacters
            .Include(ic => ic.DefaultPose)
            .Include(ic => ic.Poses)
            .ThenInclude(p => p.Pose)
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction(character);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var character = await context.InteractionCharacters
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.InteractionCharacters.Remove(character);
    }

    public async Task DeletePoseAsync(Guid id, CancellationToken ct = default)
    {
        var character = await context.InteractionCharacterPoses
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.InteractionCharacterPoses.Remove(character);
    }
}
