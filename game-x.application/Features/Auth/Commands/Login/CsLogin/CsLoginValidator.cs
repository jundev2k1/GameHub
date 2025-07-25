namespace game_x.application.Features.Auth.Commands.Login.CsLogin;

public sealed class CsLoginValidator : AbstractValidator<CsLoginCommand>
{
    public CsLoginValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage($"{nameof(CsLoginCommand.UserName)} is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(CsLoginCommand.Password)} is required.");
    }
}
