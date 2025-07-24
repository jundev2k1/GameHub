namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record StaffOrderStatusDto(string UserId, Guid OrderId, string Status);
