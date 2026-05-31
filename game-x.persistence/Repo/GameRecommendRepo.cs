using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Games.Dtos;

namespace game_x.persistence.Repo;

public sealed class GameRecommendRepo(GameXContext context) : IGameRecommendRepo, IRepository
{
    public async Task<GameRecommendDto[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameRecommends
            .AsNoTracking()
            .Select(gr => new GameRecommendDto
            {
                LocalId = gr.Id,
                Id = gr.PublicId,
                Name = gr.Name,
                Description = gr.Description ?? string.Empty,
                BannerId = gr.BannerId,
                Type = gr.Type,
                Status = gr.Status,
                StartDate = gr.StartDate,
                EndDate = gr.EndDate,
                CreatedAt = gr.CreatedAt,
                UpdatedAt = gr.UpdatedAt,
                Items = gr.Items.Select(i => new GameRecommendItemDto
                {
                    LocalGameId = i.GameId,
                    Priority = i.Priority,
                    CustomTitle = i.CustomTitle,
                    IsActive = i.IsActive,
                    IsGameActive = i.Game.IsActive,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                }).ToArray()
            })
            .ToArrayAsync(ct);
    }
    
    public async Task<GameRecommend> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await context.GameRecommends
            .AsNoTracking()
            .Include(gr => gr.Items)
            .ThenInclude(gri => gri.Game)
            .ThenInclude(g => g.Platform)
            .FirstOrDefaultAsync(gr => gr.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task<GameRecommend?> GetOverlapItemAsync(GameRecommend recommend, CancellationToken ct = default)
    {
        var minDate = DateTime.MinValue;
        var maxDate = DateTime.MaxValue;
        return await context.GameRecommends
            .AsNoTracking()
            .FirstOrDefaultAsync(
                gr => gr.PublicId != recommend.PublicId
                    && (recommend.Type == gr.Type)
                    && (recommend.StartDate ?? minDate) < (gr.EndDate ?? maxDate)
                    && (recommend.EndDate ?? maxDate) > (gr.StartDate ?? minDate),
                ct);
    }

    public async Task AddAsync(GameRecommend recommend, CancellationToken ct = default)
    {
        await context.GameRecommends.AddAsync(recommend, ct);
    }

    public async Task AddItemsAsync(Guid id, IEnumerable<GameRecommendItem> items, CancellationToken ct = default)
    {
        await context.GameRecommendItems.AddRangeAsync(items, ct);
    }

    public async Task UpdateAsync(Guid id, Func<GameRecommend, Task> updateAction, CancellationToken ct = default)
    {
        var targetRecommend = await context.GameRecommends
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetRecommend);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var targetRecommend = await context.GameRecommends
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.GameRecommends.Remove(targetRecommend);
    }

    public async Task DeleteAllItemsAsync(Guid id, CancellationToken ct = default)
    {
        var targetRecommends = await context.GameRecommendItems
            .Where(i => i.GameRecommend.PublicId == id)
            .ToArrayAsync(ct);

        context.RemoveRange(targetRecommends);
    }
}
