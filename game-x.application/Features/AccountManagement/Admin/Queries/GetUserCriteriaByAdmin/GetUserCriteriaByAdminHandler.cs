using System.Linq.Expressions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.application.Mappers.User;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetUserCriteriaByAdmin;

public sealed class GetUserCriteriaByAdminHandler(
    IAppUserRepo appUserRepo,
    ICriteriaBuilder<UserMappingDto> builder,
    UserMapper userMapper)
    : IQueryHandler<GetUserCriteriaByAdminQuery, PaginationResult<UserDto>>
{
    private readonly Dictionary<string, Func<object, Expression<Func<UserMappingDto, bool>>>> options =
        new()
        {
            ["createdByName"] = value => user => user.Staff != null && user.Staff.UserName == value.ToString(),
            ["createdById"] = value => user => user.Staff != null && user.Staff.Id == value.ToString(),
            ["passportNumber"] = value =>
                user => user.Passport != null && user.Passport.PassportNumber == value.ToString()
        };

    public async Task<PaginationResult<UserDto>> Handle(GetUserCriteriaByAdminQuery request,
        CancellationToken ct = default)
    {
        var items = await appUserRepo.GetUserByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user =>
                        (user.UserName != null && user.UserName.Contains(keyword)) ||
                        (user.Passport!.PassportNumber != null && user.Passport.PassportNumber.Contains(keyword)),
                options)
            ,
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var result = userMapper.ToUserDtos(items);
        return result;
    }
}