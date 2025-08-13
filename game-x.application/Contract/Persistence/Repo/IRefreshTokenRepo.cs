namespace game_x.application.Contract.Persistence.Repo;

public interface IRefreshTokenRepo
{
    Task<RefreshToken?> GetValidTokenAsync(string tokenHash, CancellationToken ct = default);

    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    Task RevokeByIdAsync(Guid publicId, CancellationToken ct = default);
}
