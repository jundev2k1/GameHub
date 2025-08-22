using System.Linq.Expressions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Admin.Queries.GetUserCriteriaByAdmin;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetUserCriteriaByAdmin;

public sealed class GetUserCriteriaByAdminHandler(
    IUserRepo userRepo,
    ICriteriaBuilder<UserDto> builder
    )
    : IQueryHandler<GetUserCriteriaByAdminQuery, PaginationResult<UserDto>>
{
    private readonly Dictionary<string, Func<object, Expression<Func<UserDto, bool>>>> _options =
        new()
        {
            // ["createdByName"] = value => user => user.Staff != null && user.Staff.UserName == value.ToString(),
            // ["createdById"] = value => user => user.Staff != null && user.Staff.Id == value.ToString(),
            // ["passportNumber"] = value =>
            //     user => user.Passport != null && user.Passport.PassportNumber == value.ToString()
        };

    public async Task<PaginationResult<UserDto>> Handle(GetUserCriteriaByAdminQuery request,
        CancellationToken ct = default)
    {
        var items = await userRepo.GetUserByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user =>
                        (user.Nickname != null && user.Nickname.Contains(keyword)) ||
                        (user.UserName != null && user.UserName.Contains(keyword)) ||
                        (user.Email != null && user.Email.Contains(keyword)),
                _options)
            ,
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        // var result = userMapper.ToUserDtos(items);
        return items;
    }
}