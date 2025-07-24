namespace game_x.application.Features.Auth.Commands.Login.StaffLogin;

public sealed class StaffLoginValidator : AbstractValidator<StaffLoginCommand>
{
    public StaffLoginValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage($"{nameof(StaffLoginCommand.UserName)} is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(StaffLoginCommand.Password)} is required.");
        RuleFor(x => x.CounterId)
            .NotEmpty().WithMessage($"{nameof(StaffLoginCommand.CounterId)} is required.");
    }
}
