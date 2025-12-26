namespace game_x.application.Features.UserGameSessions.Services;

public interface IUserSessionTrackingService
{
    Task CheckInAsync(
        string userId,
        string connectionId,
        Guid platformId,
        Guid? gameId, CancellationToken ct = default);

    Task<bool> PingAsync(string connectionId, CancellationToken ct = default);

    Task CheckOutAsync(string connectionId, CancellationToken ct = default);
}
