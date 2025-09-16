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
    public async Task<PaginationResult<FriendDto>> GetFriendshipsAsync(
        string userId,
        Func<IQueryable<FriendDto>, IQueryable<FriendDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var friendLinks = context.SocialLinks
            .AsNoTracking()
            .Where(sl => 
                sl.Kind == SocialLinkKind.Friendship &&
                sl.State == SocialLinkState.Accepted &&
                (sl.UserIdMin == userId || sl.UserIdMax == userId))
            .Select(sl => new {
                sl.PublicId,
                sl.RespondedAt,
                OtherUserId = sl.UserIdMin == userId ? sl.UserIdMax : sl.UserIdMin});

        var query = friendLinks.Join(
            context.Users.AsNoTracking(),
            x => x.OtherUserId,
            u => u.Id,
            (x, u) => new FriendDto{
                UserId = u.Id,
                Nickname = u.Nickname,
                RespondedAt = x.RespondedAt,
                LinkId = x.PublicId,
                AvatarUrl = String.Empty
            }).AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<FriendDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<PaginationResult<SocialLinkDto>> GetIncomingRequestsByCriteriaAsync(
        string addresseeUserId,
        Func<IQueryable<SocialLinkDto>, IQueryable<SocialLinkDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = context.SocialLinks
            .AsNoTracking()
            .Include(sl => sl.AddresseeUser)
            .Include(sl => sl.RequesterUser)
            .Where(sl => 
                sl.Kind == SocialLinkKind.Friendship &&
                sl.State == SocialLinkState.Pending &&
                sl.AddresseeUserId == addresseeUserId)
            .AsQueryable();

        var query = baseQuery
            .Select(x => x.Adapt<SocialLinkDto>());

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
    
    public async Task<PaginationResult<SocialLinkDto>> GetOutgoingRequestsByCriteriaAsync(
        string requesterUserId,
        Func<IQueryable<SocialLinkDto>, IQueryable<SocialLinkDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = context.SocialLinks
            .AsNoTracking()
            .Include(x => x.AddresseeUser)
            .Include(x => x.RequesterUser)
            .Where(x => 
                x.Kind == SocialLinkKind.Friendship &&
                x.State == SocialLinkState.Pending &&
                x.RequesterUserId == requesterUserId)
            .AsQueryable();

        var query = baseQuery
            .Select(x => x.Adapt<SocialLinkDto>());

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
    
    public async Task<SocialLink> UpdateAsync(Guid publicId, Action<SocialLink> updateAction, CancellationToken ct = default)
    {
        var link = await context.SocialLinks
                       .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
                   ?? throw new NotFoundException(MessageCode.Chatting.SocialLinkNotFound);

        updateAction.Invoke(link);
        await context.SaveChangesAsync(ct);
        return link;
    }

}