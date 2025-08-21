namespace game_x.application.Features.ChainTransactions.Dtos;

public sealed class DepositChainTransactionResponseDto
{
    public Guid TransactionId { get; set; } =Guid.Empty;
    public decimal Amount { get; set; }
    public string To { get; set; } = String.Empty;
}
