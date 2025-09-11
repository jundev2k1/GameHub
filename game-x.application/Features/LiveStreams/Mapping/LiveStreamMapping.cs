using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.Games.Mapping;

public static class LiveStreamMapping
{
    public static PaginationResult<LiveStreamScheduleListItemDto> ToSearchResult(this PaginationResult<LivestreamSchedule> data)
    {
        var result = new PaginationResult<LiveStreamScheduleListItemDto>(
            items: [.. data.Items.Select(i => i.Adapt<LiveStreamScheduleListItemDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages, 
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}