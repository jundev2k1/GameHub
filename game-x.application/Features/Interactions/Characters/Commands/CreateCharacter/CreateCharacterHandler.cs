namespace game_x.application.Features.Interactions.Characters.Commands.CreateCharacter;

public sealed class CreateCharacterHandler : ICommandHandler<CreateCharacterCommand>
{
    public async Task<Unit> Handle(CreateCharacterCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
