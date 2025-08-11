using game_x.share.Extensions;

namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public sealed class DecisionBankAccountValidator : AbstractValidator<DecisionBankAccountCommand>
{
    public DecisionBankAccountValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage($"{nameof(DecisionBankAccountCommand.UserId)} must be not empty.");

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

    private bool BeValidStatus(KycStatus @enum)
    {
        return @enum == KycStatus.Approved || @enum == KycStatus.Rejected;
    }

    private static bool BeRequiredField(KycStatus status, string? input)
    {
        return status != KycStatus.Rejected
            || (status == KycStatus.Rejected && input.IsNotNullOrEmpty());
    }
}
