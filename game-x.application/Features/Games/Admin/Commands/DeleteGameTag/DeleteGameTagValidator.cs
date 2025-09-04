namespace game_x.application.Features.Games.Admin.Commands.DeleteGameTag;

public sealed class DeleteGameTagValidator : AbstractValidator<DeleteGameTagCommand>
{
    public DeleteGameTagValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Tag ID is required.");
    }
}
