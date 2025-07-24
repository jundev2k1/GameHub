using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Staff.Queries.GetSelfStaff;

public sealed class GetSelfStaffHandler(IAppUserRepo appUserRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetSelfStaffQuery, StaffDetailDto>
{
    public async Task<StaffDetailDto> Handle(GetSelfStaffQuery request, CancellationToken ct = default)
    {
        var staffId = userAccessor.GetUserId();

        var userDetail = await appUserRepo.GetStaffDetailByIdAsync(staffId, ct);

        if (userDetail is null)
            throw new NotFoundException(nameof(AppUser), nameof(AppUser.Id));

        return userDetail.Adapt<StaffDetailDto>();
    }
}