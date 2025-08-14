using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class UserBankAccountRepo(GameXContext context) : IUserBankAccountRepo, IRepository
{
    public async Task<UserBankAccount[]> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var result = await context.UserBankAccounts
            .AsNoTracking()
            .Where(uba => uba.UserId == userId
                && uba.User.Status == UserStatus.Active
                && uba.User.IsDeleted == false)
            .ToArrayAsync(ct)
            ?? throw new NotFoundException(nameof(UserBankAccount), userId);
        return result;
    }

    public async Task<UserBankAccount> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await context.UserBankAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(uba => uba.PublicId == id
                && uba.User.Status == UserStatus.Active
                && uba.User.IsDeleted == false
                && uba.FiatCurrency.IsActive, ct)
            ?? throw new NotFoundException(nameof(UserBankAccount), id);
        return result;
    }

    public async Task<UserBankAccount> GetByCurencyCodeAsync(string userId, CurrencyUnit curencyCode, CancellationToken ct = default)
    {
        var result = await context.UserBankAccounts
            .AsNoTracking()
            .Include(uba => uba.Image)
            .Include(uba => uba.FiatCurrency)
            .Include(uba => uba.ReviewedBy)
            .FirstOrDefaultAsync(uba => uba.UserId == userId
                && uba.FiatCurrency.Code == curencyCode
                && uba.User.Status == UserStatus.Active
                && uba.User.IsDeleted == false
                && uba.FiatCurrency.IsActive, ct)
            ?? throw new NotFoundException(nameof(UserBankAccount), curencyCode.Value);
        return result;
    }

    public async Task UpdateAsync(Guid id, Action<UserBankAccount> updateAction, CancellationToken ct = default)
    {
        var targetItem = await context.UserBankAccounts
            .Include(uba => uba.Image)
            .FirstOrDefaultAsync(uba =>
                uba.PublicId == id
                && uba.User.IsDeleted == false
                && uba.User.Status == UserStatus.Active, ct)
            ?? throw new NotFoundException(nameof(UserBankAccount), id);

        updateAction(targetItem);
    }
    public async Task UpdateAsync(string userId, CurrencyUnit currencyCode, Action<UserBankAccount> updateAction, CancellationToken ct = default)
    {
        var targetItem = await context.UserBankAccounts
            .Include(uba => uba.Image)
            .FirstOrDefaultAsync(uba =>
                uba.UserId == userId
                && uba.User.IsDeleted == false
                && uba.User.Status == UserStatus.Active
                && uba.FiatCurrency.Code == currencyCode, ct)
            ?? throw new NotFoundException(nameof(UserBankAccount), userId);

        updateAction(targetItem);
    }
}
