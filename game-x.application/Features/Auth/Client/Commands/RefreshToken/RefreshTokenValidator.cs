namespace game_x.application.Features.Auth.Client.Commands.RefreshToken;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.ExpiredAccessToken)
            .NotEmpty()
            .WithMessage($"{nameof(RefreshTokenCommand.ExpiredAccessToken)} is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage($"{nameof(RefreshTokenCommand.RefreshToken)} is required.");
    }
}
