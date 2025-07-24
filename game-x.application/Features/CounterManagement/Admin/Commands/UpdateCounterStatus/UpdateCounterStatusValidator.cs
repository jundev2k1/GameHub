namespace game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounterStatus;

public sealed class UpdateCounterStatusValidator : AbstractValidator<UpdateCounterStatusCommand>
{
    public UpdateCounterStatusValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status.");
    }
}