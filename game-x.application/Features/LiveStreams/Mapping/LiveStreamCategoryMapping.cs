using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Mapping;

public static class LiveStreamCategoryMapping
{
    public static PaginationResult<LiveStreamCategoryListItemDto> ToSearchResult(this PaginationResult<LiveStreamCategory> data)
    {
        var result = new PaginationResult<LiveStreamCategoryListItemDto>(
            items: [.. data.Items.Select(i => i.Adapt<LiveStreamCategoryListItemDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages, 
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}