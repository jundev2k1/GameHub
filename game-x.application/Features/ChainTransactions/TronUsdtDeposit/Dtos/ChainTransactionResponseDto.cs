namespace game_x.application.Features.ChainTransactions.TronUsdtDeposit.Dtos;

public sealed class CreateChainTransactionResponseDto
{
    public string OrderUid { get; set; } = default!;
    public decimal Amount { get; set; } = default!;
    public string To { get; set; }
}
