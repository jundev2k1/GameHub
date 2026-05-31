namespace game_x.application.Features.UserRoles.Admin.Queries;

public record GetAllRoleQuery: IQuery<GetAllRoleResult[]>;

public record GetAllRoleResult(string Id, string Name);
