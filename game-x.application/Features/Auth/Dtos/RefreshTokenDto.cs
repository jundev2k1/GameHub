namespace game_x.application.Features.Auth.Dtos;

public sealed class RefreshTokenDto
{
    public Guid PublicId { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public string TokenHash { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public SyncState State { get; set; } = SyncState.NotSynced;
    public bool IsSynced => State == SyncState.Synced;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => ExpiresAt < DateTime.UtcNow;

    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum SyncState
{
    NotSynced,
    Synced
}
