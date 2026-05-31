using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Root.Queries.GetAdminById;

public sealed class GetAdminByIdHandler(IUserRepo userRepo, IAuthService authService)
    : IQueryHandler<GetAdminByIdQuery, AdminDto>
{
    public async Task<AdminDto> Handle(GetAdminByIdQuery request, CancellationToken ct = default)
    {
        // Get target user
        var userDetail = await userRepo.GetUserByIdAsync(request.AdminId, ct);

        // Check role user
        var role = await authService.GetRolesAsync(userDetail);
        if (!role.IsAdmin) throw new ForbiddenException(MessageCode.System.Forbidden);

        return userDetail.Adapt<AdminDto>();
    }
}
