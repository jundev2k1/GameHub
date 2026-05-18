namespace game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Withdrawal;

public record FastPayWithdrawalResponse(
    string OrderUid,
    decimal Amount,
    string? To);