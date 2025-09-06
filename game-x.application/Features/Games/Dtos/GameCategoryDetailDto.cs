namespace game_x.application.Features.Games.Dtos;

public sealed class GameCategoryDetailDto : GameCategoryDto
{
    public CategoryRelatedGameDto[] RelatedGames { get; set; } = [];
}

public sealed class CategoryRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
