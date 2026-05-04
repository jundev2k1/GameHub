using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.DepositSuccess;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayDepositSuccess;

public record FastPayDepositSuccessCommand(
    DepositSucessCallbackRequest Data,
    string Signature) : ICommand;
