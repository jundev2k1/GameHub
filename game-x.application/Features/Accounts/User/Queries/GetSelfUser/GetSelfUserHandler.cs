using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public sealed class GetSelfUserHandler(IUserRepo appUserRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetSelfUserQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetSelfUserQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var userDetail = await appUserRepo.GetUserByIdAsync(userId, ct);
        return userDetail.Adapt<UserDetailDto>();
    }
}
