namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientBalanceDto(
    Guid BalanceId,
    decimal Amount,
    decimal FrozenAmount);