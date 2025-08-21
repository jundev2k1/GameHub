namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientWalletsDto(
    ClientCryptoBalanceDto[] SiteBalances,
    decimal? GameBalance);