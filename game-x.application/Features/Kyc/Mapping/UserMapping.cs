using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Features.Kyc.Mapping;

public static class UserMapping
{
    public static PaginationResult<UserKycListItemDto> ToSearchResult(this PaginationResult<UserKyc> data)
    {
        var result = new PaginationResult<UserKycListItemDto>(
            items: [.. data.Items.Select(item => item.Adapt<UserKycListItemDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
