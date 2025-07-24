using System.Linq.Expressions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.application.Mappers.Staff;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetStaffCriteriaByAdmin;

public sealed class GetStaffCriteriaByAdminHandler(
    IAppUserRepo appUserRepo,
    ICriteriaBuilder<StaffMappingDto> builder,
    StaffMapper staffMapper
)
    : IQueryHandler<GetStaffCriteriaByAdminQuery, PaginationResult<StaffDto>>
{
    private readonly Dictionary<string, Func<object, Expression<Func<StaffMappingDto, bool>>>> options =
        new()
        {
            ["createdByName"] = value => user => user.Admin != null && user.Admin.UserName == value.ToString(),
            ["createdById"] = value => user => user.Admin != null && user.Admin.Id == value.ToString()
        };

    public async Task<PaginationResult<StaffDto>> Handle(GetStaffCriteriaByAdminQuery request,
        CancellationToken ct = default)
    {
        var items = await appUserRepo.GetStaffByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user => user.UserName!.StartsWith(keyword), options),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var result = staffMapper.ToStaffDtos(items);
        return result;
    }
}