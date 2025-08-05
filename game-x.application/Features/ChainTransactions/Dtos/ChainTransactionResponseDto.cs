namespace game_x.application.Features.ChainTransactions.Dtos;

public sealed class CreateChainTransactionResponseDto
{
    public string OrderUid { get; set; } = String.Empty;
    public decimal Amount { get; set; }
    public string To { get; set; } = String.Empty;
}
