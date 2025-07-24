using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class UserPassportRepo(GameXContext context) : IUserPassportRepo
{
    public async Task<UserPassport> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await context.UserPassport
            .AsNoTracking()
            .Include(p => p.PassportImage)
            .FirstOrDefaultAsync(p => p.AppUserId == userId, ct)
            ?? throw new NotFoundException(nameof(userId), userId);
    }

    public async Task<UserPassport> GetByPassportNumberAsync(string passportNumber, CancellationToken ct = default)
    {
        return await context.UserPassport
            .AsNoTracking()
            .Include(p => p.PassportImage)
            .FirstOrDefaultAsync(p => p.PassportNumber == passportNumber, ct)
            ?? throw new NotFoundException(nameof(passportNumber), passportNumber);
    }

    public async Task<bool> IsExistsByPassportNumberAsync(string passportNumber, CancellationToken ct = default)
        => await context.UserPassport.AsNoTracking().AnyAsync(p => p.PassportNumber == passportNumber, ct);

    public async Task AddAsync(UserPassport passport, CancellationToken ct = default)
    {
        await context.UserPassport.AddAsync(passport, ct);
    }

    public async Task UpdatePassportAsync(string passportNumber, Action<UserPassport> updateAction, CancellationToken ct = default)
    {
        var targetPassport = await context.UserPassport
            .FirstOrDefaultAsync(passport => passport.PassportNumber == passportNumber, ct)
            ?? throw new NotFoundException(nameof(passportNumber), passportNumber);

        updateAction?.Invoke(targetPassport);
    }
}
