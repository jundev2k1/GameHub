namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.TokenId)
            .NotNull().WithMessage($"{nameof(RevokeTokenCommand.TokenId)} is required.")
            .NotEmpty().WithMessage($"{nameof(RevokeTokenCommand.TokenId)} must be not empty.");
    }
}
