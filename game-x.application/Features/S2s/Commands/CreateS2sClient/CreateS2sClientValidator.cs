namespace game_x.application.Features.S2s.Commands.CreateS2sClient;

public sealed class CreateS2sClientValidator : AbstractValidator<CreateS2sClientCommand>
{
    public CreateS2sClientValidator()
    {
        RuleFor(x => x.ClientName)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(255).WithMessage("Client name must be not greater than or equal 255 characters.");

        RuleFor(x => x.ClientCode)
            .NotNull().WithMessage("ClientCode must be not null.")
            .MaximumLength(255).WithMessage("ClientCode must be not greater than or equal 255 characters.");

        RuleFor(x => x.Notes)
            .NotNull().WithMessage("Note must be not null.")
            .MaximumLength(4000).WithMessage("Notes must be not greater than or equal 4000 characters.");
    }
}
