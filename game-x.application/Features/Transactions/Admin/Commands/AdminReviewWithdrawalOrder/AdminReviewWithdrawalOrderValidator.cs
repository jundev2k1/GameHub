namespace game_x.application.Features.Transactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderValidator : AbstractValidator<AdminReviewWithdrawalOrderCommand>
{
    public AdminReviewWithdrawalOrderValidator()
    {
        RuleFor(x => x.OrderStatus)
            .Must(status => new[] { TransactionStatus.Approved, TransactionStatus.Rejected }.Contains(status))
            .WithMessage("OrderStatus is invalid.");
    }
}