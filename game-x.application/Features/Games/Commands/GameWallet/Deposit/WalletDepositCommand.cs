namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public record WalletDepositCommand(
    decimal Amount) : ICommand;