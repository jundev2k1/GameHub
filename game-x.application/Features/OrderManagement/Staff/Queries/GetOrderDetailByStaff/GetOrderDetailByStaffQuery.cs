using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Staff.Queries.GetOrderDetailByStaff;

public record GetOrderDetailByStaffQuery(Guid OrderId) : IQuery<OrderDetailInfoDto>;