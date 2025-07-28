namespace game_x.application.Features.Auth.Root.Commands.RootLogin;

public sealed class RootLoginValidator : AbstractValidator<RootLoginCommand>
{
    public RootLoginValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage($"{nameof(RootLoginCommand.UserName)} is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(RootLoginCommand.Password)} is required.");
    }
}
