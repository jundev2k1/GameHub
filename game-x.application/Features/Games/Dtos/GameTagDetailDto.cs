namespace game_x.application.Features.Games.Dtos;

public sealed class GameTagDetailDto : GameTagDto
{
    public GameTagRelatedGameDto[] RelatedGames { get; set; } = [];
}

public sealed class GameTagRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlatformName { get; set; } = string.Empty;
}
