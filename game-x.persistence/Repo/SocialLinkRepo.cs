using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Friends.Dtos;
using game_x.domain.Constants;

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
                Avatar = u.Avatar,
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
    
    public async Task<PaginationResult<FriendSearchDto>> SearchFriendshipsAsync(
        string userId,
        Func<IQueryable<FriendSearchDto>, IQueryable<FriendSearchDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        // Exclude blockers or blocked users
        var blockedUserIds =
            context.SocialLinks.AsNoTracking()
                .Where(sl =>
                    (sl.UserIdMin == userId || sl.UserIdMax == userId) &&
                    sl.Kind == SocialLinkKind.Block)
                .Select(sl => sl.UserIdMin == userId ? sl.UserIdMax : sl.UserIdMin);
        
        var friendLinks =
            context.SocialLinks
                .AsNoTracking()
                .Where(sl => sl.UserIdMin == userId || sl.UserIdMax == userId)
                .Select(sl => new
                {
                    sl.PublicId,
                    sl.Kind,
                    sl.State,
                    sl.RequesterUserId,
                    sl.AddresseeUserId,
                    sl.CreatedAt,
                    OtherUserId = sl.UserIdMin == userId ? sl.UserIdMax : sl.UserIdMin
                });

        var query = context.Users
            .AsNoTracking()
            .Where(u => u.UserRoles.Any(ur => ur.Role.NormalizedName == AppRoles.User.ToUpper()))
            .Where(u => u.Id != userId && !blockedUserIds.Contains(u.Id))
            .GroupJoin(
                friendLinks,
                u => u.Id,
                fl => fl.OtherUserId,
                (u, gj) => new { u, gj })
            .SelectMany(
                x => x.gj.DefaultIfEmpty(),
                (x, fl) => new FriendSearchDto
                {
                    UserId = x.u.Id,
                    Nickname = x.u.Nickname,
                    Email = x.u.Email!,
                    Avatar = x.u.Avatar,
                    LinkId = fl == null ? null : fl.PublicId,
                    State = fl == null ? null : fl.State,
                    RequesterUserId = fl == null ? null : fl.RequesterUserId,
                    AddresseeUserId = fl == null ? null : fl.AddresseeUserId,
                    LinkCreatedAt = fl == null ? null : fl.CreatedAt
                });

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<FriendSearchDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<PaginationResult<SocialLink>> GetIncomingRequestsByCriteriaAsync(
        string addresseeUserId,
        Func<IQueryable<SocialLink>, IQueryable<SocialLink>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = context.SocialLinks
            .AsNoTracking()
            .Include(sl => sl.RequesterUser)
                .ThenInclude(x => x!.Avatar)
            .Where(sl => 
                sl.Kind == SocialLinkKind.Friendship &&
                sl.State == SocialLinkState.Pending &&
                sl.AddresseeUserId == addresseeUserId)
            .AsQueryable();

        var query = baseQuery
            .Select(x => new SocialLink
                {
                PublicId = x.PublicId,
                Kind = x.Kind,
                State = x.State,
                RequesterUserId = x.RequesterUserId,
                RequesterUser = x.RequesterUser,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });
            
        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<SocialLink>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<PaginationResult<SocialLink>> GetOutgoingRequestsByCriteriaAsync(
        string requesterUserId,
        Func<IQueryable<SocialLink>, IQueryable<SocialLink>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = context.SocialLinks
            .AsNoTracking()
            .Include(x => x.AddresseeUser)
                .ThenInclude(x => x!.Avatar)
            .Where(x => 
                x.Kind == SocialLinkKind.Friendship &&
                x.State == SocialLinkState.Pending &&
                x.RequesterUserId == requesterUserId)
            .AsQueryable();

        var query = baseQuery
            .Select(x => new SocialLink
            {
                PublicId = x.PublicId,
                Kind = x.Kind,
                State = x.State,
                AddresseeUserId = x.AddresseeUserId,
                AddresseeUser = x.AddresseeUser,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<SocialLink>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<PaginationResult<SocialLink>> GetBlockedFriendsAsync(
        string userId,
        Func<IQueryable<SocialLink>, IQueryable<SocialLink>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = context.SocialLinks
            .AsNoTracking()
            .Include(x => x.BlockedUser)
                .ThenInclude(u => u!.Avatar)
            .Where(x => x.Kind == SocialLinkKind.Block && x.BlockerUserId == userId)
            .AsQueryable();

        var query = baseQuery
            .Select(x => new SocialLink
            {
                PublicId = x.PublicId,
                Kind = x.Kind,
                State = x.State,
                BlockedUserId = x.BlockedUserId,
                BlockedUser = x.BlockedUser,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                RespondedAt = x.RespondedAt
            });

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<SocialLink>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<SocialLink?> GetByKeyPairAsync(string min, string max, CancellationToken ct = default)
    {
        return await context.SocialLinks
            .AsNoTracking()
            .Include(l => l.AddresseeUser)
                .ThenInclude(x => x!.Avatar)
            .Include(l => l.RequesterUser)
                .ThenInclude(x => x!.Avatar)
            .Include(l => l.BlockedUser)
                .ThenInclude(x => x!.Avatar)
            .Include(l => l.BlockerUser)
                .ThenInclude(x => x!.Avatar)
            .FirstOrDefaultAsync(l => 
                l.UserIdMin == min &&
                l.UserIdMax == max, ct);
    }
    
    public async Task<SocialLink> GetByIdAsync(Guid linkId, CancellationToken ct = default)
    {
        return await context.SocialLinks
            .AsNoTracking()
            .Include(l => l.AddresseeUser)
                .ThenInclude(x => x!.Avatar)
            .Include(l => l.RequesterUser)
                .ThenInclude(x => x!.Avatar)
            .Include(l => l.BlockedUser)
                .ThenInclude(x => x!.Avatar)
            .Include(l => l.BlockerUser)
                .ThenInclude(x => x!.Avatar)
            .FirstOrDefaultAsync(l => l.PublicId == linkId, ct)
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