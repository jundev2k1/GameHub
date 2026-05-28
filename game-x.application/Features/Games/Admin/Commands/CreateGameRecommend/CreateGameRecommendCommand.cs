namespace game_x.application.Features.Games.Admin.Commands.CreateGameRecommend;

public record CreateGameRecommendCommand(
    string Name,
    string Description,
    RecommendationType Type,
    PublishStatus Status,
    DateTime? StartDate,
    DateTime? EndDate,
    CreateGameRecommendItemDto[] Items) : ICommand;

public record CreateGameRecommendItemDto(
    Guid GameId,
    int Priority = 0,
    string? CustomTitle = null,
    bool IsActive = true);
