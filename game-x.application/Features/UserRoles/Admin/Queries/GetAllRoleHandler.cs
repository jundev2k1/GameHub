using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.UserRoles.Admin.Queries;

public class GetAllRolesQueryHandler(IRoleRepo roleRepo): IQueryHandler<GetAllRoleQuery, GetAllRoleResult[]>
{
    public async Task<GetAllRoleResult[]> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepo.GetAllAsync();
        return [.. roles.Select(r => new GetAllRoleResult(r.Id, r.Name))];
    }
}
