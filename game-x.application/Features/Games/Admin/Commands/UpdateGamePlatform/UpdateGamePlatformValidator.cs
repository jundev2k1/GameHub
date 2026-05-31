namespace game_x.application.Features.Games.Admin.Commands.UpdateGamePlatform;

public sealed class UpdateGamePlatformValidator : AbstractValidator<UpdateGamePlatformCommand>
{
    public UpdateGamePlatformValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateGamePlatformCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGamePlatformCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGamePlatformCommand.Note)} must be less than 4000 chacracters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(UpdateGamePlatformCommand.Priority)} must be greater than or equal 0.");
    }
}
