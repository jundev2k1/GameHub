using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.User.Queries.GetSelfUser;

public sealed class GetSelfUserHandler(IAppUserRepo appUserRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetSelfUserQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetSelfUserQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();

        var userDetail = await appUserRepo.GetUserDetailByIdAsync(userId, ct);

        if (userDetail is null)
            throw new NotFoundException(nameof(AppUser), nameof(AppUser.Id));

        return userDetail.Adapt<UserDetailDto>();
    }
}