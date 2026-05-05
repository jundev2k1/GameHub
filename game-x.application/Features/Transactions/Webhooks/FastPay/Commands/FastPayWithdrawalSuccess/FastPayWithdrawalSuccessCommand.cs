using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.DepositSuccess;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayWithdrawalSuccess;

public record FastPayWithdrawalSuccessCommand(
    DepositSucessCallbackRequest Data,
    string Signature) : ICommand;
