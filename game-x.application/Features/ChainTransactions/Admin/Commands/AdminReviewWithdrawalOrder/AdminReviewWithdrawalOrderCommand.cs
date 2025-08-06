namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public class AdminReviewWithdrawalOrderCommand : ICommand
{
    public Guid OrderId { get; set; }
    public required ChainTransactionStatus OrderStatus { get; set; }
}