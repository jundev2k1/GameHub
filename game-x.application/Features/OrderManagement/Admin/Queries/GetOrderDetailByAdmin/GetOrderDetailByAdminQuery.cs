using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Admin.Queries.GetOrderDetailByAdmin;

public record GetOrderDetailByAdminQuery(Guid OrderId) : IQuery<OrderDetailInfoDto>;