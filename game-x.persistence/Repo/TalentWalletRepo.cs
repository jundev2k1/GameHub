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
            .Join(dbContext.LiveStreamDonations,
                twt => twt.ReferenceId,
                lsd => lsd.PublicId,
                (twt, lsd) => new { twt, lsd })
            .Join(dbContext.SystemWalletTransactions,
                gr => gr.twt.ReferenceId,
                swt => swt.ReferenceId,
                (gr, swt) => new TalentWalletTransactionDto
                {
                    Id = gr.twt.PublicId,
                    TalentId = gr.twt.TalentId,
                    Nickname = gr.twt.TalentWallet.Talent.Nickname,
                    Amount = gr.twt.Amount,
                    Type = gr.twt.Type,
                    ReferenceId = gr.twt.ReferenceId,
                    DonorNickname = gr.lsd.Donor.Nickname,
                    DonorEmail = gr.lsd.Donor.Email,
                    DonatedAmount = gr.lsd.Amount,
                    TalentReceive = gr.twt.Amount,
                    SystemReceive = swt != null ? swt.Amount : 0,
                    BalanceAfter = gr.twt.BalanceAfter,
                    AdjustedBy = gr.twt.AdjustedBy,
                    CreatedAt = gr.twt.CreatedAt,
                    UpdatedAt = gr.twt.UpdatedAt,
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
