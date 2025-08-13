namespace game_x.domain.Entities;

public sealed class RefreshToken : BaseEntity<long>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();

    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;

    public string TokenHash { get; private set; } = string.Empty;
    public string JwtId { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public string? DeviceInfo { get; private set; }
    public string? Location { get; private set; }

    public static RefreshToken Create(
        string userId,
        string tokenHash,
        string jwtId,
        DateTime expiresAt,
        string ipAddress,
        string userAgent,
        string? deviceInfo = null,
        string? location = null)
    {
        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            JwtId = jwtId,
            ExpiresAt = expiresAt.ToUniversalTime(),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeviceInfo = deviceInfo,
            Location = location
        };
    }

    public void Revoke()
    {
        if (IsRevoked())
            throw new InvalidOperationException("Refresh token already revoked.");

        RevokedAt = DateTime.UtcNow;
    }

    public bool IsActive() => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    public bool IsRevoked() => RevokedAt != null;
}
