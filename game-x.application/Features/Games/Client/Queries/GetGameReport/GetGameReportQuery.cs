using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Client.Queries.GetGameReport;

public record GetGameReportQuery(
    [property: JsonIgnore] Guid PlatformId,
    DateTime? StartDate,
    DateTime? EndDate) : IQuery<Unit>;
