namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderValidator : AbstractValidator<AdminReviewWithdrawalOrderCommand>
{
    public AdminReviewWithdrawalOrderValidator()
    {
        RuleFor(x => x.OrderStatus)
            .Must(status => new[] { ChainTransactionStatus.Approved, ChainTransactionStatus.Rejected }.Contains(status))
            .WithMessage("OrderStatus is invalid.");
    }
}