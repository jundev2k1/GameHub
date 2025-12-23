namespace game_x.application.Features.UserGameSessions.Dtos;

public sealed class GameHubTokenDto
{
    public Guid GamePlatformId { get; set; }
    public Guid GameId { get; set; }
}
