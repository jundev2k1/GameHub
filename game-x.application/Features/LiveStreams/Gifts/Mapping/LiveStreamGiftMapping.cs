using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Mapping;

public static class LiveStreamGiftMapping
{
    public static PaginationResult<LiveStreamGiftDto> ToSearchResult(
        this PaginationResult<LiveStreamGift> data,
        (Guid PublicId, string? Icon)[] giftFiles)
    {
        var result = new PaginationResult<LiveStreamGiftDto>(
            items: [.. data.Items
                .Select(item =>
                {
                    var (publicId, icon) = giftFiles.FirstOrDefault(i => i.PublicId == item.PublicId);
                    var dto = item.Adapt<LiveStreamGiftDto>();
                    dto.IconUrl = icon;
                    return dto;
                })],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}