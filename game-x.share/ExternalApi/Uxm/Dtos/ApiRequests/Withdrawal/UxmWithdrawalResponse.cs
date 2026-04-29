namespace game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Withdrawal;

public record UxmWithdrawalResponse(
    string OrderUid,
    decimal Amount,
    string? To);