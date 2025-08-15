namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientWalletsDto(
    ClientCryptoBalanceDto[] SiteBalances,
    decimal? GameBalance);

public record ClientCryptoBalanceDto(
    decimal Amount,
    decimal FrozenAmount,
    decimal TotalAmount,
    string Network,
    string Symbol);