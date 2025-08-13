namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage($"{nameof(RevokeTokenCommand.Id)} is required.");
    }
}
