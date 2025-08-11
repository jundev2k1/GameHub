namespace game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;

public record GetSelfUserBalanceQuery : IQuery<IEnumerable<GetSelfUserBalanceResult>>;

public record GetSelfUserBalanceResult(
    Guid Id,
    string UserId,
    string CryptoTokenId,
    decimal Amount,
    decimal FrozenAmount,
    decimal TotalAmount);