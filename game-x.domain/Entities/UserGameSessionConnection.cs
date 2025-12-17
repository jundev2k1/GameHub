namespace game_x.domain.Entities;

public sealed class UserGameSessionConnection : BaseEntity<long>
{
    public int UserGameSessionId { get; private set; }
    public UserGameSession Session { get; private set; } = default!;

    public string ConnectionId { get; private set; } = string.Empty;
    public DateTime ConnectedAt { get; private set; }
    public DateTime? DisconnectedAt { get; private set; }

    public static UserGameSessionConnection Create(
        int sessionId,
        string connectionId)
    {
        return new UserGameSessionConnection
        {
            UserGameSessionId = sessionId,
            ConnectionId = connectionId,
            ConnectedAt = DateTime.UtcNow
        };
    }

    public void Disconnect()
    {
        if (DisconnectedAt != null)
            throw new ArgumentException("This Session is ended.");

        DisconnectedAt = DateTime.UtcNow;
    }
}
