namespace game_x.application.Features.Transactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrderV2;

public sealed class AdminReviewWithdrawalOrderV2Validator : AbstractValidator<AdminReviewWithdrawalOrderV2Command>
{
    public AdminReviewWithdrawalOrderV2Validator()
    {
        RuleFor(x => x.OrderStatus)
            .Must(status => new[] { TransactionStatus.Approved, TransactionStatus.Rejected }.Contains(status))
            .WithMessage("OrderStatus is invalid.");
    }
}