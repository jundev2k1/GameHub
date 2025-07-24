using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Staff.Queries.GetUserDetailByStaff;

public record GetUserDetailByStaffQuery(string UserId) : IQuery<UserDetailDto>;
