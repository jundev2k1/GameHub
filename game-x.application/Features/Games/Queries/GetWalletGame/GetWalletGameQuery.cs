namespace game_x.application.Features.Games.Queries.WalletGame;

public record GetWalletGameQuery : ICommand<GetWalletGameResult>;

public record GetWalletGameResult(string Currency, decimal Quota);
