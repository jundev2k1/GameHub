using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionFailed;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayWithdrawalFailed;

public record FastPayWithdrawalFailedCommand(
    TransactionFailedRequest Data,
    string Signature) : ICommand;
