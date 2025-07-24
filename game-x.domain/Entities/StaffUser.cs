namespace game_x.domain.Entities;

public sealed class StaffUser : BaseEntity<int>
{
    public int CounterId { get; private set; }
    public Counter Counter { get; private set; } = default!;
    public string StaffId { get; private set; } = string.Empty;
    public AppUser Staff { get; private set; } = default!;
    public string UserId { get; private set; } = string.Empty;
    public AppUser User { get; private set; } = default!;

    public static StaffUser Create(int counterId, string staffId, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(staffId, nameof(staffId));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        var counter = new StaffUser
        {
            CounterId = counterId,
            StaffId = staffId,
            UserId = userId,
        };
        return counter;
    }
}
