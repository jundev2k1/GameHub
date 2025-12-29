using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameRecommend;

public record UpdateGameRecommendCommand(
    [property: JsonIgnore] Guid? Id,
    string Name,
    string Description,
    PublishStatus Status,
    DateTime? StartDate,
    DateTime? EndDate,
    UpdateGameRecommendItemDto[] Items) : ICommand;

public record UpdateGameRecommendItemDto(
    Guid GameId,
    int Priority = 0,
    string? CustomTitle = null,
    bool IsActive = true);
