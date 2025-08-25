using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Deposit;

public record WalletDepositCommand(decimal Amount, Guid CryptoTokenId, string? Note) : ICommand<GameTransactionDto>;