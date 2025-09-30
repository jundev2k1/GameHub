using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Interactions.Characters.Dtos;

namespace game_x.application.Features.Interactions.Characters.Mapping;

public static class InteractionCharacterMapping
{
    public static PaginationResult<InteractionCharacterListItemDto> ToSearchResult(this PaginationResult<InteractionCharacter> data)
    {
        var result = new PaginationResult<InteractionCharacterListItemDto>(
            items: [.. data.Items.Select(i => i.Adapt<InteractionCharacterListItemDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages, 
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}