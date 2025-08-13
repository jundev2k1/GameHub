namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public record WalletWithdrawalCommand(
    decimal Amount) : ICommand;
