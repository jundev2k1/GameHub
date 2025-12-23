using game_x.share.Extensions;

namespace game_x.application.Features.Kyc.Commands._2_DecisionKyc;

public sealed class DecisionKycValidator : AbstractValidator<DecisionKycCommand>
{
    public DecisionKycValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage($"{nameof(DecisionKycCommand.UserId)} must be not empty.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage($"{nameof(DecisionKycCommand.Status)} must be in enum.")
            .Must(BeValidStatus).WithMessage($"{nameof(DecisionKycCommand.Status)} is invalid status.");

        RuleFor(x => x.Details)
            .Must((model, value) => BeRequiredField(model.Status, value))
            .WithMessage($"{nameof(DecisionKycCommand.Details)} must be not empty.");
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
