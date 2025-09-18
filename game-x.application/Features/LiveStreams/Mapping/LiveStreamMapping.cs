using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Mapping;

public static class LiveStreamMapping
{
    public static PaginationResult<LiveStreamScheduleListItemDto> ToSearchResult(
        this PaginationResult<LivestreamSchedule> data,
        (Guid Id, string? Url)[] avatars)
    {
        LiveStreamScheduleListItemDto MapToDto(LivestreamSchedule schedule)
        {
            var dto = schedule.Adapt<LiveStreamScheduleListItemDto>();
            if (dto.AssignedTo is null) return dto;

            var (Id, Url) = avatars.FirstOrDefault(avt => avt.Id == dto.Id);
            dto.AssignedTo.Avatar = Url;

            return dto;
        }
        ;

        var result = new PaginationResult<LiveStreamScheduleListItemDto>(
            items: [.. data.Items.Select(MapToDto)],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}