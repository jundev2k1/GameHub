namespace game_x.application.Features.S2s.Commands.UpdateS2sClient;

public sealed class UpdateS2sClientValidator : AbstractValidator<UpdateS2sClientCommand>
{
    public UpdateS2sClientValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client id is required.");

        RuleFor(x => x.ClientName)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(255).WithMessage("Client name must be not greater than 255 characters.");

        RuleFor(x => x.Notes)
            .NotNull().WithMessage("Note must be not null.")
            .MaximumLength(4000).WithMessage("Notes must be not greater than 4000 characters.");
    }
}
