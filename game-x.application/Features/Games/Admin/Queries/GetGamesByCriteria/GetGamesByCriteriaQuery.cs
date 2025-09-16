using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;

namespace game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;

public record GetGamesByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<GetGamesByCriteriaListItem>>;


public class GetGamesByCriteriaListItem
{
    public Guid Id { get; set; }
    public string GameCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
