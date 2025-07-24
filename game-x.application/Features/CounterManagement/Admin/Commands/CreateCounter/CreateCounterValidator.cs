namespace game_x.application.Features.CounterManagement.Admin.Commands.CreateCounter;

public sealed class CreateCounterValidator : AbstractValidator<CreateCounterCommand>
{
    public CreateCounterValidator()
    {
        RuleFor(x => x.CounterName).NotEmpty().WithMessage("Counter name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Location).NotEmpty().WithMessage("Location by is required.");
    }
}
