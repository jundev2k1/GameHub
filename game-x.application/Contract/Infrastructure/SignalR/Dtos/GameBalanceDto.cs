namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record GameBalanceDto(
    decimal Amount,
    decimal FrozenAmount,
    NetworkType Network);