namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record GameBalanceNotificationDto(
    decimal Amount,
    decimal FrozenAmount,
    string Network,
    string Symbol);