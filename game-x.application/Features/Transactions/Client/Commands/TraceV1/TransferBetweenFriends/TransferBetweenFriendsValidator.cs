
namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TransferBetweenFriends;

public sealed class TransferBetweenFriendsValidator : AbstractValidator<TransferBetweenFriendsCommand>
{
    public TransferBetweenFriendsValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage($"{nameof(TransferBetweenFriendsCommand.Amount)} is required.")
            .GreaterThanOrEqualTo(10).WithMessage($"{nameof(TransferBetweenFriendsCommand.Amount)} must be at least 10 USDT.");

        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage($"{nameof(TransferBetweenFriendsCommand.TargetUserId)} is required.");
        
        RuleFor(x => x.Note)
            .MaximumLength(200).WithMessage($"{nameof(TransferBetweenFriendsCommand.Note)} cannot exceed 200 characters.");
    }
}