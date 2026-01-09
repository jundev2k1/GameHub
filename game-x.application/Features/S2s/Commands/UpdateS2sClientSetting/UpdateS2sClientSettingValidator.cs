namespace game_x.application.Features.S2s.Commands.UpdateS2sClientSetting;

public sealed class UpdateS2sClientSettingValidator : AbstractValidator<UpdateS2sClientSettingCommand>
{
    public UpdateS2sClientSettingValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client id is required.");

        RuleFor(x => x.AppName)
            .NotEmpty().WithMessage("App name is required.")
            .MaximumLength(255).WithMessage("App name must be not greater than 255 characters.");

        RuleFor(x => x.Notes)
            .NotNull().WithMessage("Note must be not null.")
            .MaximumLength(4000).WithMessage("Notes must be not greater than 4000 characters.");
    }
}
