using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionCompleted;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayWithdrawalSuccess;

public record FastPayWithdrawalSuccessCommand(
    TransactionCompletedRequest Data,
    string Signature) : ICommand;
