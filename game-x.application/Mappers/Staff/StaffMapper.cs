using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Mappers.Staff;

public sealed class StaffMapper()
{
    public PaginationResult<StaffDto> ToStaffDtos(PaginationResult<StaffMappingDto> data)
    {
        var result = new PaginationResult<StaffDto>(
            data.Items.Select(item => item.Adapt<StaffDto>()).ToList(),
            data.TotalItems,
            data.TotalPages,
            data.PageNumber,
            data.PageSize);
        return result;
    }
}