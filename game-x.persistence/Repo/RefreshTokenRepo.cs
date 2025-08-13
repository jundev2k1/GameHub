using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class RefreshTokenRepo(GameXContext context) : IRefreshTokenRepo, IRepository
{
    public async Task<RefreshToken?> GetValidTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.TokenHash == tokenHash
                && rt.ExpiresAt > DateTime.UtcNow
                && rt.RevokedAt == null)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => await context.RefreshTokens.AddAsync(token, ct);

    public async Task RevokeByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.PublicId == publicId, ct)
            ?? throw new NotFoundException("Refresh token not found.");

        token.Revoke();
    }
}
