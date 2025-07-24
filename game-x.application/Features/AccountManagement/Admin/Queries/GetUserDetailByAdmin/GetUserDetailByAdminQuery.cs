using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetUserDetailByAdmin;

public record GetUserDetailByAdminQuery(string UserId) : IQuery<UserDetailDto>;