using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class BankAccountRepo(GameXContext context) : IBankAccountRepo
{
    public async Task<BankAccount[]> GetsByOwnerIdAsync(string ownerId, CancellationToken ct = default)
    {
        return await context.BankAccounts
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync(ct);
    }

    public async Task<BankAccount> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.BankAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(ba => ba.Id == id, ct)
            ?? throw new NotFoundException(nameof(BankAccount), nameof(BankAccount.Id));
    }
    public async Task<BankAccount> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.BankAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(ba => ba.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(BankAccount), nameof(BankAccount.Id));
    }

    public async Task<bool> IsExistsAsync(string ownerId, string accountNumber, CancellationToken ct = default)
    {
        return await context.BankAccounts
            .AsNoTracking()
            .AnyAsync(ba => (ba.OwnerId == ownerId) && (ba.BankAccountNumber == accountNumber), ct);
    }

    public async Task AddAsync(BankAccount bankAccount, CancellationToken ct = default)
    {
        await context.AddAsync(bankAccount, ct);
    }

    public async Task UpdateAsync(Guid bankAccountCode, string ownerId, Action<BankAccount> updateAction, CancellationToken ct = default)
    {
        var targetBankAccount = await context.BankAccounts
            .FirstOrDefaultAsync(ba => (ba.PublicId == bankAccountCode) && (ba.OwnerId == ownerId), ct)
            ?? throw new NotFoundException(nameof(BankAccount), nameof(BankAccount.Id));

        updateAction?.Invoke(targetBankAccount);
    }

    public async Task UpdateDefaultAccountAsync(Guid bankAccountId, string ownerId, CancellationToken ct = default)
    {
        var bankAccounts = await context.BankAccounts
            .Where(ba => ba.OwnerId == ownerId)
            .ToListAsync(ct);
        if (bankAccounts.Any(ba => ba.PublicId == bankAccountId) == false)
            throw new NotFoundException(nameof(BankAccount), nameof(BankAccount.PublicId));

        bankAccounts.ForEach(bankAccount => bankAccount.UpdateIsDefault(bankAccountId));
    }

    public async Task DeleteAsync(Guid bankAccountCode, string ownerId, CancellationToken ct = default)
    {
        var targetBankAccount = await context.BankAccounts
            .FirstOrDefaultAsync(bankAccount => (bankAccount.PublicId == bankAccountCode)
                && (bankAccount.OwnerId == ownerId), ct)
            ?? throw new NotFoundException(nameof(BankAccount), nameof(BankAccount.Id));

        context.Remove(targetBankAccount);
    }

    public async Task<PaginationResult<BankAccount>> GetBankAccountByCriteriaAsync(
        string userId,
        Func<IQueryable<BankAccount>, IQueryable<BankAccount>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.BankAccounts
            .AsNoTracking()
            .Where(ba => ba.OwnerId == userId)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<BankAccount>(
            items: items,
            totalItems: totalCount,
            totalPages: (int)Math.Ceiling((decimal)totalCount / pageSize),
            pageIndex: page,
            pageSize: pageSize);
    }
}
