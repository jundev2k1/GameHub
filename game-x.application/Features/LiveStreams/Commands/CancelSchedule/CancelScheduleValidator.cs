namespace game_x.application.Features.LiveStreams.Commands.CancelSchedule;

public sealed class CancelScheduleValidator : AbstractValidator<CancelScheduleCommand>
{
    public CancelScheduleValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(255).WithMessage("Reason cannot exceed 255 characters.");
    }
}
