using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionCompleted;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayDepositSuccess;

public record FastPayDepositSuccessCommand(
    TransactionCompletedRequest Data,
    string Signature,
    string RawRequest) : ICommand;
