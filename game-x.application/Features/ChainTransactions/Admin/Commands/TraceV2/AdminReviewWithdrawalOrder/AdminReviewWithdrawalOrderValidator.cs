namespace game_x.application.Features.ChainTransactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderValidator : AbstractValidator<AdminReviewWithdrawalOrderCommand>
{
    public AdminReviewWithdrawalOrderValidator()
    {
        RuleFor(x => x.OrderStatus)
            .Must(status => new[] { TransactionStatus.Approved, TransactionStatus.Rejected }.Contains(status))
            .WithMessage("OrderStatus is invalid.");
    }
}