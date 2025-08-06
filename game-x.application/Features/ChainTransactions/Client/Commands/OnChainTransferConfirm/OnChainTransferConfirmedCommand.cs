namespace game_x.application.Features.ChainTransactions.Client.Commands.OnChainTransferConfirm;

public class OnChainTransferConfirmedCommand : ICommand<Unit>
{
    public required Guid PublicId { get; set; }
    public required string OrderNumber { get; set; }
    public required string UserId { get; init; }
    public required int CryptoTokenId { get; init; }
    public required decimal Amount { get; init; }
    public required string FromAddress { get; init; }
    public required string ToAddress { get; init; }
    public required bool Result { get; set; }
    public required ChainTransactionType Type { get; init; }
}
