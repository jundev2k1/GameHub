namespace game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Deposit;

public record FastPayDepositResponse(string OrderUid, decimal Amount, string To);
