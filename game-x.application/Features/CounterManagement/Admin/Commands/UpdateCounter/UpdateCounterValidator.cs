namespace game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounter;

public sealed class UpdateCounterValidator : AbstractValidator<UpdateCounterCommand>
{
    public UpdateCounterValidator()
    {
        RuleFor(x => x.CounterName)
            .NotEmpty()
            .WithMessage("CounterName must not empty.")
            .MaximumLength(500)
            .WithMessage("CounterName must not exceed 500 characters.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status is invalid.");

        RuleFor(x => x.Location)
            .MaximumLength(500)
            .WithMessage("Location must not exceed 500 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}