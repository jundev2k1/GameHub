using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;

public record GetUserDetailByAdminQuery(string UserId) : IQuery<UserDetailDto>;
