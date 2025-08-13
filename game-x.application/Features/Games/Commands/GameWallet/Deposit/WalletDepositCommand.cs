using System.Text.Json.Serialization;
using MediatR;

namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public record WalletDepositCommand(
    decimal Amount) : IRequest;
