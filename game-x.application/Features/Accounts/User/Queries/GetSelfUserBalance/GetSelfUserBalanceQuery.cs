namespace game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;

public record GetSelfUserBalanceQuery : IQuery<IEnumerable<GetSelfUserBalanceResult>>;

public class GetSelfUserBalanceResult
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public NetworkType NetWork { get; set; }
    public decimal Amount { get; set; }
    public decimal FrozenAmount { get; set; }
    public decimal TotalAmount { get; set; }
}