using game_x.share.ExternalApi.Etl998.Dtos.Wallet;

namespace game_x.application.Features.Games.Client.Commands.Etl998.AccountBalance;

public record AccountBalanceCommand(
    string AccountName, 
    string Password) : ICommand<IReadOnlyCollection<Etl998WalletResponse>>;