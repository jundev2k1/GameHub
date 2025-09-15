using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Friends.Dtos;
using game_x.domain.Constants;
using Mapster;

namespace game_x.persistence.Repo;

public class SocialLinkRepo(GameXContext context): ISocialLinkRepo, IRepository
{
    public async Task<PaginationResult<SocialLinkDto>> GetRequestsByCriteriaAsync(
        string addresseeUserId,
        Func<IQueryable<SocialLinkDto>, IQueryable<SocialLinkDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = context.SocialLinks
            .Include(u => u.AddresseeUser)
            .Include(u => u.RequesterUser)
            .Where(u => 
                u.Kind == SocialLinkKind.Friendship &&
                u.State == SocialLinkState.Pending &&
                u.AddresseeUserId == addresseeUserId)
            .AsQueryable();

        var query = baseQuery
            .Select(u => u.Adapt<SocialLinkDto>());

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<SocialLinkDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<SocialLink?> GetByKeyPairAsync(string min, string max, SocialLinkKind kind, CancellationToken ct = default)
    {
        return await context.SocialLinks
            .AsNoTracking()
            .Include(l => l.AddresseeUser)
            .Include(l => l.RequesterUser)
            .FirstOrDefaultAsync(l => 
                l.UserIdMin == min && 
                l.UserIdMax == max && 
                l.Kind == kind, ct);
    }
    
    public async Task<SocialLink> GetByIdAsync(Guid linkId, SocialLinkKind kind, CancellationToken ct = default)
    {
        return await context.SocialLinks
            .AsNoTracking()
            .Include(l => l.AddresseeUser)
            .Include(l => l.RequesterUser)
            .FirstOrDefaultAsync(l => l.PublicId == linkId && l.Kind == kind, ct)
        ?? throw new NotFoundException(MessageCode.Chatting.SocialLinkNotFound);
    }
    
    public async Task AddAsync(SocialLink link, CancellationToken ct = default)
    {
        await context.SocialLinks.AddAsync(link, ct);
    }
    
    public async Task PatchUpdateAsync(Guid publicId, Action<SocialLink> updateAction, CancellationToken ct = default)
    {
        var conv = await context.SocialLinks
                       .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
                   ?? throw new NotFoundException(MessageCode.Chatting.SocialLinkNotFound);

        updateAction.Invoke(conv);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task PutUpdateAsync(SocialLink link, CancellationToken ct = default)
    {
        context.Entry(link).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}