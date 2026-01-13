using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.TalentWallets.DTOs;
using Mapster;

namespace game_x.persistence.Repo;

public sealed class TalentWalletRepo(GameXContext dbContext) : ITalentWalletRepo, IRepository
{
    public async Task<PaginationResult<TalentWalletTransactionDto>> GetsByCriteriaAsync(
        string? talentId = null,
        Func<IQueryable<TalentWalletTransactionDto>, IQueryable<TalentWalletTransactionDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = dbContext.TalentWalletTransactions
            .AsNoTracking()
            .Join(dbContext.SystemWalletTransactions,
                twt => twt.ReferenceId,
                swt => swt.ReferenceId,
                (twt, swt) => new TalentWalletTransactionDto
                {
                    Id = twt.PublicId,
                    TalentId = twt.TalentId,
                    Nickname = twt.TalentWallet.Talent.Nickname,
                    Amount = twt.Amount,
                    Type = twt.Type,
                    ReferenceId = twt.ReferenceId,
                    TalentReceive = twt.Amount,
                    SystemReceive = swt != null ? swt.Amount : 0,
                    BalanceAfter = twt.BalanceAfter,
                    AdjustedBy = twt.AdjustedBy,
                    CreatedAt = twt.CreatedAt,
                    UpdatedAt = twt.UpdatedAt,
                })
            .AsQueryable();
        if (talentId != null)
            query = query.Where(q => q.TalentId == talentId);

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => i.Adapt<TalentWalletTransactionDto>())
            .ToListAsync(ct);

        var donationIds = items.Select(i => i.ReferenceId)
            .ToArray();
        var mappingDatas = await dbContext.LiveStreamDonations
            .AsNoTracking()
            .Include(lsd => lsd.Donor)
            .Where(lsd => donationIds.Contains(lsd.PublicId))
            .ToArrayAsync(ct);
        items.ForEach(i =>
        {
            var targetMapping = mappingDatas.FirstOrDefault(m => m.PublicId == i.ReferenceId);
            if (targetMapping != null)
            {
                i.DonorNickname = targetMapping.Donor.Nickname;
                i.DonorEmail = targetMapping.Donor.Email;
                i.DonatedAmount = targetMapping.Amount;
            }
        });
        return new PaginationResult<TalentWalletTransactionDto>(
            items,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<TalentWallet> GetWalletAsync(string userId, CancellationToken ct = default)
    {
        return await dbContext.TalentWallets.AsNoTracking().FirstOrDefaultAsync(tw => tw.Id == userId, ct)
            ?? throw new NotFoundException(nameof(userId), userId);
    }

    public async Task UpdateAsync(string userId, Action<TalentWallet> updateAction, CancellationToken ct = default)
    {
        var target = await dbContext.TalentWallets
            .FirstOrDefaultAsync(tw => tw.Id == userId, ct)
            ?? throw new NotFoundException(nameof(userId), userId);

        updateAction.Invoke(target);
    }
}
