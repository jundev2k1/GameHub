namespace game_x.application.Features.Auth.Commands.Login.AdminLogin;

public sealed class AdminLoginValidator : AbstractValidator<AdminLoginCommand>
{
    public AdminLoginValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage($"{nameof(AdminLoginCommand.UserName)} is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(AdminLoginCommand.Password)} is required.");
    }
}
