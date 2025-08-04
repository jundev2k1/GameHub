using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForRegistration;

public sealed class VerifyEmailForRegistrationValidator : AbstractValidator<VerifyEmailForRegistrationCommand>
{
    public VerifyEmailForRegistrationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailForRegistrationCommand.Email)} must be not empty.")
            .IsEmail(nameof(VerifyEmailForRegistrationCommand.Email));

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailForRegistrationCommand.Code)} must be not empty.")
            .Length(8).WithMessage($"{nameof(VerifyEmailForRegistrationCommand.Code)} must be 8 characters.");
    }
}
