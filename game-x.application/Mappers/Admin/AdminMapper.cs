using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Mappers.Admin;

public sealed class AdminMapper()
{
    public PaginationResult<AdminDto> ToAdminDtos(PaginationResult<AppUser> data)
    {
        var result = new PaginationResult<AdminDto>(
            data.Items.Select(item => item.Adapt<AdminDto>()).ToList(),
            data.TotalItems,
            data.TotalPages,
            data.PageNumber,
            data.PageSize);
        return result;
    }
}
