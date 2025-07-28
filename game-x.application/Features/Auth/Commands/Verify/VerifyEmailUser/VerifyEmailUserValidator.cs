using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Commands.Verify.VerifyEmailUser;

public sealed class VerifyEmailUserValidator : AbstractValidator<VerifyEmailUserCommand>
{
    public VerifyEmailUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailUserCommand.Email)} must be not empty.")
            .IsEmail(nameof(VerifyEmailUserCommand.Email));

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailUserCommand.Code)} must be not empty.")
            .Length(8).WithMessage($"{nameof(VerifyEmailUserCommand.Code)} must be 8 characters.");
    }
}
