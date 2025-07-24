using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetUserDetailByAdmin;

public sealed class GetUserDetailByAdminHandler(IAppUserRepo appUserRepo)
    : IQueryHandler<GetUserDetailByAdminQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserDetailByAdminQuery request, CancellationToken ct = default)
    {
        var userDetail = await appUserRepo.GetUserDetailByIdAsync(request.UserId, ct);

        if (userDetail is null)
            throw new NotFoundException(nameof(AppUser), nameof(AppUser.Id));

        return userDetail.Adapt<UserDetailDto>();
    }
}