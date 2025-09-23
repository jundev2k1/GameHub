using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamChatRepo(GameXContext context) : ILiveStreamChatRepo, IRepository
{
    public async Task<PaginationResult<LiveStreamChatMessage>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamChatMessage>, IQueryable<LiveStreamChatMessage>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.LiveStreamChatMessages
            .AsNoTracking()
            .Include(ls => ls.LiveStream)
            .Include(ls => ls.Sender)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<LiveStreamChatMessage>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<LiveStreamChatMessage> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.LiveStreamChatMessages
            .AsNoTracking()
            .Include(ls => ls.LiveStream)
            .Include(ls => ls.Sender)
            .FirstOrDefaultAsync(ls => ls.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task CreateAsync(LiveStreamChatMessage message, CancellationToken ct = default)
    {
        await context.LiveStreamChatMessages.AddAsync(message, ct);
    }

    public async Task DeleteAsync(Guid messageId, CancellationToken ct = default)
    {
        var targetMessage = await context.LiveStreamChatMessages
            .FirstOrDefaultAsync(ls => ls.PublicId == messageId, ct)
            ?? throw new NotFoundException(nameof(messageId), messageId);

        context.LiveStreamChatMessages.Remove(targetMessage);
    }
}
