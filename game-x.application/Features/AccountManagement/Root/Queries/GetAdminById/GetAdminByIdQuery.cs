using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Root.Queries.GetAdminById;

public record GetAdminByIdQuery(string AdminId) : IQuery<AdminDto>;