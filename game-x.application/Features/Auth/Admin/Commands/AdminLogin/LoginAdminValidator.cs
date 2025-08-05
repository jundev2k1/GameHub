namespace game_x.application.Features.Auth.Admin.Commands.AdminLogin;

public sealed class LoginAdminValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage($"{nameof(LoginAdminCommand.UserName)} is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(LoginAdminCommand.Password)} is required.");
    }
}
