using game_x.application.Common.Abstractions.Time;

namespace game_x.infrastructure.Common.Time;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}