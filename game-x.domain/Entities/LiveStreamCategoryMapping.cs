namespace game_x.domain.Entities;

public sealed class LiveStreamCategoryMapping : BaseEntity<object>
{
    public int ScheduleId { get; private set; }
    public LivestreamSchedule Schedule { get; private set; } = default!;

    public int CategoryId { get; private set; }
    public LiveStreamCategory Category { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static LiveStreamCategoryMapping Create(int scheduleId, int categoryId, bool isPrimary = false, int priority = 0)
    {
        return new LiveStreamCategoryMapping
        {
            ScheduleId = scheduleId,
            CategoryId = categoryId,
            IsPrimary = isPrimary,
            Priority = priority,
        };
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void SetPriority(int priority)
    {
        if (priority < 0)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority cannot be negative.");

        Priority = priority;
    }
}
