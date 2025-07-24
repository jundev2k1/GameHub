using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Mappers.User;

public sealed class UserMapper()
{
    public PaginationResult<UserDto> ToUserDtos(PaginationResult<UserMappingDto> data)
    {
        var result = new PaginationResult<UserDto>(
            data.Items.Select(item => item.Adapt<UserDto>()).ToList(),
            data.TotalItems,
            data.TotalPages,
            data.PageNumber,
            data.PageSize);
        return result;
    }
}
