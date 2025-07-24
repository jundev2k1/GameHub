using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Root.Queries.GetAdminById;

public sealed class GetAdminByIdHandler(IAppUserRepo appUserRepo)
    : IQueryHandler<GetAdminByIdQuery, AdminDto>
{
    public async Task<AdminDto> Handle(GetAdminByIdQuery request, CancellationToken ct = default)
    {
        var userDetail = await appUserRepo.GetAdminByIdAsync(request.AdminId, ct);

        if (userDetail is null)
            throw new NotFoundException(nameof(AppUser), nameof(AppUser.Id));

        return userDetail.Adapt<AdminDto>();
    }
}