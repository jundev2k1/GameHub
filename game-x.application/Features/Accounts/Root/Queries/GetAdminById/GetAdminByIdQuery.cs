using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Root.Queries.GetAdminById;

public record GetAdminByIdQuery(string AdminId) : IQuery<AdminDto>;
