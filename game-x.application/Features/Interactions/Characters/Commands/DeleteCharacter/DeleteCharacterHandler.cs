namespace game_x.application.Features.Interactions.Characters.Commands.DeleteCharacter;

public sealed class DeleteCharacterHandler : ICommandHandler<DeleteCharacterCommand>
{
    public async Task<Unit> Handle(DeleteCharacterCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
