namespace game_x.application.Contract.Persistence.Repo;

public interface IRefreshTokenRepo
{
    Task<RefreshToken[]> GetActiveTokensAsync(CancellationToken ct = default);

    Task<RefreshToken?> FindActiveByTokenHashAsync(string tokenHash, CancellationToken ct = default);

    Task<RefreshToken?> FindByIdAsync(Guid publicId, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<RefreshToken> tokens, CancellationToken ct = default);

    Task BulkRevokeAsync(IEnumerable<Guid> publicIds, DateTime revokedAtUtc, CancellationToken ct = default);
}
