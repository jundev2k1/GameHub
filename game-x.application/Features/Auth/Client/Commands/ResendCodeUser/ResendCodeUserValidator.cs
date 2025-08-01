using game_x.application.Extensions;
using game_x.application.Services.Verification;

namespace game_x.application.Features.Auth.Client.Commands.ResendCodeUser;

public sealed class ResendCodeUserValidator : AbstractValidator<ResendCodeUserCommand>
{
    public ResendCodeUserValidator()
    {
        RuleFor(x => x.Purpose)
            .Must(p => p == VerificationPurposes.ChangePassword
                || p == VerificationPurposes.EmailVerification
                || p == VerificationPurposes.ForgotPassword)
            .WithMessage("Invalid purpose. Must be 'ForgotPassword' or 'EmailVerification' or 'ChangePassword'.");

        When(x => x.Purpose == VerificationPurposes.ChangePassword, () =>
        {
            RuleFor(x => x.Email)
                .Must(email => email == null)
                .WithMessage("Email must be null or empty when purpose is 'ForgotPassword'.");
        });

        var requireEmailPurposes = new[] { VerificationPurposes.EmailVerification, VerificationPurposes.ForgotPassword };
        When(x => requireEmailPurposes.Contains(x.Purpose), () =>
        {
            RuleFor(x => x.Email!)
                .NotEmpty().WithMessage("Email is required when purpose is 'EmailVerification'.")
                .IsEmail();
        });
    }
}
