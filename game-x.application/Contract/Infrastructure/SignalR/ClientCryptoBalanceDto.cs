namespace game_x.application.Contract.Infrastructure.SignalR;

public record ClientCryptoBalanceDto(
    decimal Amount,
    decimal FrozenAmount,
    decimal TotalAmount,
    NetworkType Network,
    string Symbol);