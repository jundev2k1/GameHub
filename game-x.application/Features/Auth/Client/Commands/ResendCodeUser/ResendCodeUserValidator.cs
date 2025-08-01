using game_x.application.Extensions;
using game_x.application.Services.Verification;

namespace game_x.application.Features.Auth.Client.Commands.ResendCodeUser;

public sealed class ResendCodeUserValidator : AbstractValidator<ResendCodeUserCommand>
{
    public ResendCodeUserValidator()
    {
        RuleFor(x => x.Purpose)
            .Must(p => p == VerificationPurposes.ForgotPassword || p == VerificationPurposes.EmailVerification)
            .WithMessage("Invalid purpose. Must be 'ForgotPassword' or 'EmailVerification'.");

        When(x => x.Purpose == VerificationPurposes.ForgotPassword, () =>
        {
            RuleFor(x => x.Email)
                .Must(email => email == null)
                .WithMessage("Email must be null or empty when purpose is 'ForgotPassword'.");
        });

        When(x => x.Purpose == VerificationPurposes.EmailVerification, () =>
        {
            RuleFor(x => x.Email!)
                .NotEmpty().WithMessage("Email is required when purpose is 'EmailVerification'.")
                .IsEmail();
        });
    }
}
