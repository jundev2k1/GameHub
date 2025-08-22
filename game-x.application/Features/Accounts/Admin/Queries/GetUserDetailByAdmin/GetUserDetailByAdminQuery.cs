using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;

public sealed class GetUserDetailByAdminHandler(IUserRepo userRepo)
    : IQueryHandler<GetUserDetailByAdminQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserDetailByAdminQuery request, CancellationToken ct = default)
    {
        var userDetail = await userRepo.GetUserDetailAsync(request.UserId, ct);

        if (userDetail is null)
            throw new NotFoundException(MessageCode.User.UserNotFound);

        return userDetail;
    }
}
