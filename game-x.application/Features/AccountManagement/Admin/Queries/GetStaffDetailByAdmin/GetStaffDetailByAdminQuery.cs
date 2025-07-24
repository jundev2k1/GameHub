using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetStaffDetailByAdmin;

public record GetStaffDetailByAdminQuery(string StaffId) : IQuery<StaffDetailDto>;