namespace game_x.domain.Entities;

public sealed class StaffCounter : BaseEntity<int>
{
    public string UserId { get; private set; } = string.Empty;
    public AppUser User { get; private set; } = default!;
    public int CounterId { get; private set; } = default!;
    public Counter Counter { get; private set; } = default!;
    public DateTime LoginAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LogoutAt { get; private set; }

    public static StaffCounter Create(
        string userId,
        int counterId,
        DateTime? loginAt = null,
        DateTime? logoutAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        var counter = new StaffCounter
        {
            UserId = userId,
            CounterId = counterId,
            LoginAt = loginAt ?? DateTime.UtcNow,
            LogoutAt = logoutAt,
        };
        return counter;
    }

    public void UpdateLogout(DateTime? logoutAt = null)
    {
        LogoutAt = logoutAt ?? DateTime.UtcNow;
    }
}
