using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Client.Queries.GetOrderDetailByClient;

public record GetOrderDetailByClientQuery(Guid OrderId) : IQuery<OrderDetailInfoDto>;