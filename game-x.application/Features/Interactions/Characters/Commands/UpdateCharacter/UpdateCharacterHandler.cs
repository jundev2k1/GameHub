namespace game_x.application.Features.Interactions.Characters.Commands.UpdateCharacter;

public sealed class UpdateCharacterHandler : ICommandHandler<UpdateCharacterCommand>
{
    public async Task<Unit> Handle(UpdateCharacterCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
