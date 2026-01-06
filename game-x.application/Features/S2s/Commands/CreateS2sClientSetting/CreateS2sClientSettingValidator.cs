namespace game_x.application.Features.S2s.Commands.CreateS2sClientSetting;

public sealed class CreateS2sClientSettingValidator : AbstractValidator<CreateS2sClientSettingCommand>
{
    public CreateS2sClientSettingValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required.");

        RuleFor(x => x.AppCode)
            .NotEmpty().WithMessage("App code is required.")
            .MaximumLength(50).WithMessage("App code must be not greater or equal than 50 characters.");

        RuleFor(x => x.AppName)
            .NotEmpty().WithMessage("App name is required.")
            .MaximumLength(255).WithMessage("App name must be not greater or equal than 255 characters.");

        RuleFor(x => x.Notes)
            .NotNull().WithMessage("Note must be not null.")
            .MaximumLength(4000).WithMessage("Notes must be not greater than or equal 4000 characters.");
    }
}
