namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientOrderStatusDto(Guid OrderId, string Status);
