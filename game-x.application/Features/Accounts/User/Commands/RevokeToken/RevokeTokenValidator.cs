namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotNull().WithMessage($"{nameof(RevokeTokenCommand.Token)} is required.")
            .NotEmpty().WithMessage($"{nameof(RevokeTokenCommand.Token)} must be not empty.");
    }
}
