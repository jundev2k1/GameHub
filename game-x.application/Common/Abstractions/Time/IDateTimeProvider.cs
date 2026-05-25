namespace game_x.application.Common.Abstractions.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}