using System.Text.Json.Serialization;
using MediatR;

namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public record WalletWithdrawalCommand(
    decimal Amount) : IRequest;
