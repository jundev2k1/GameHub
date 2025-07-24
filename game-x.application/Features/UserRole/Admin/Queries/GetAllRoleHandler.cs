using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.UserRole.Dtos;

namespace game_x.application.Features.UserRole.Admin.Queries;

public class GetAllRolesQueryHandler(IRoleRepo roleRepo): IRequestHandler<GetAllRoleQuery, List<RoleDto>>
{
    public async Task<List<RoleDto>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepo.GetAllAsync();

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();
    }
}