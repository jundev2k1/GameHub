namespace game_x.application.Features.ChainTransactions.Dtos;

public sealed class DepositChainTransactionResponseDto
{
    public decimal Amount { get; set; }
    public string To { get; set; } = String.Empty;
    public ListTransactionInternalDto Transaction { get; set; } = null!;
}
