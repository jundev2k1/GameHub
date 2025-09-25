using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Mapping;

public static class LiveStreamGiftMapping
{
    public static PaginationResult<LiveStreamGiftDto> ToSearchResult(
        this PaginationResult<LiveStreamGift> data)
    {
        var result = new PaginationResult<LiveStreamGiftDto>(
            items: [.. data.Items.Select(item => item.Adapt<LiveStreamGiftDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}