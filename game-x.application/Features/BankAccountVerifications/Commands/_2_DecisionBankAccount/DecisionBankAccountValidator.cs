using game_x.share.Extensions;

namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public sealed class DecisionBankAccountValidator : AbstractValidator<DecisionBankAccountCommand>
{
    public DecisionBankAccountValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage($"{nameof(DecisionBankAccountCommand.Status)} must be in enum.")
            .Must(BeValidStatus).WithMessage($"{nameof(DecisionBankAccountCommand.Status)} is invalid status.");

        RuleFor(x => x.Reason)
            .Must((model, value) => BeRequiredField(model.Status, value))
            .WithMessage($"{nameof(DecisionBankAccountCommand.Reason)} must be not empty.");

        RuleFor(x => x.Details)
            .Must((model, value) => BeRequiredField(model.Status, value))
            .WithMessage($"{nameof(DecisionBankAccountCommand.Details)} must be not empty.");
    }

    private bool BeValidStatus(UserBankAccountStatus @enum)
    {
        return @enum == UserBankAccountStatus.Approved || @enum == UserBankAccountStatus.Rejected;
    }

    private static bool BeRequiredField(UserBankAccountStatus status, string? input)
    {
        return status != UserBankAccountStatus.Rejected
            || (status == UserBankAccountStatus.Rejected && input.IsNotNullOrEmpty());
    }
}
