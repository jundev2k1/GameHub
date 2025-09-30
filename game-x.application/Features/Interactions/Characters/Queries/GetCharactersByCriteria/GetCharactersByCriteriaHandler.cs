using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Interactions.Characters.Dtos;
using game_x.application.Features.Interactions.Characters.Mapping;

namespace game_x.application.Features.Interactions.Characters.Queries.GetCharactersByCriteria;

public sealed class GetCharactersByCriteriaHandler(
    ICriteriaBuilder<InteractionCharacter> builder,
    IInteractionCharacterRepo characterRepo) : IQueryHandler<GetCharactersByCriteriaQuery, PaginationResult<InteractionCharacterListItemDto>>
{
    public async Task<PaginationResult<InteractionCharacterListItemDto>> Handle(GetCharactersByCriteriaQuery request, CancellationToken ct = default)
    {
        var items = await characterRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => lsc => lsc.Name.ToLowerInvariant().Contains(keyword.ToLowerInvariant())),
            request.PageIndex,
            request.PageSize,
            ct);
        return items.ToSearchResult();
    }
}
