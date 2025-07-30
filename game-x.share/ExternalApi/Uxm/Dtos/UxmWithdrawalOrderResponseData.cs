namespace game_x.share.ExternalApi.Uxm.Dtos;

public record UxmWithdrawalOrderResponseData(
    string? OrderUid,
    decimal Amount,
    string? To);