namespace game_x.application.Features.Transactions.Dtos;

public sealed class TransferTransactionDto
{
    public decimal Amount { get; set; }
    public ListTransactionInternalDto Transaction { get; set; } = null!;
}