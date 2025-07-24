using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetStaffDetailByAdmin;

public sealed class GetStaffDetailByAdminHandler(IAppUserRepo appUserRepo)
    : IQueryHandler<GetStaffDetailByAdminQuery, StaffDetailDto>
{
    public async Task<StaffDetailDto> Handle(GetStaffDetailByAdminQuery request, CancellationToken ct = default)
    {
        var userDetail = await appUserRepo.GetStaffDetailByIdAsync(request.StaffId, ct);

        if (userDetail is null)
            throw new NotFoundException(nameof(AppUser), nameof(AppUser.Id));

        return userDetail.Adapt<StaffDetailDto>();
    }
}