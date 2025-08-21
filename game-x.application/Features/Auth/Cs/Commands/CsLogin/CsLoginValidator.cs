namespace game_x.application.Features.Auth.Cs.Commands.CsLogin;

public sealed class CsLoginValidator : AbstractValidator<CsLoginCommand>
{
    public CsLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(CsLoginCommand.Email)} is required.")
            .EmailAddress().WithMessage($"{nameof(CsLoginCommand.Email)} wrong format.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(CsLoginCommand.Password)} is required.");
    }
}
