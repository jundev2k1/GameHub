namespace game_x.application.Features.Games.Dtos;

public sealed class GamePlatformDetailDto : GamePlatformDto
{
    public PlatformRelatedGameDto[] RelatedGames { get; set; } = [];
}

public sealed class PlatformRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
