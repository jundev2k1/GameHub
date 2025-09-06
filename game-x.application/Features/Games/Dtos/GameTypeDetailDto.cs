using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GameTypeDetailDto : GameTypeDto
{
    public GameTypeRelatedGameDto[] RelatedGames { get; set; } = [];
}

public sealed class GameTypeRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
