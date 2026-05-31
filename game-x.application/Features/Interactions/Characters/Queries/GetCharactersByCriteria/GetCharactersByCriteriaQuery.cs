using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Interactions.Characters.Dtos;

namespace game_x.application.Features.Interactions.Characters.Queries.GetCharactersByCriteria;

public record GetCharactersByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<InteractionCharacterListItemDto>>;
