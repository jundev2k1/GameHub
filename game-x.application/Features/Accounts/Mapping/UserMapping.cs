using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Accounts.Dtos;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Mapping;

public static class UserMapping
{
    public static PaginationResult<AdminDto> ToSearchResult(this PaginationResult<UserEntity> data)
    {
        var result = new PaginationResult<AdminDto>(
            items: [.. data.Items.Select(item => item.Adapt<AdminDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
