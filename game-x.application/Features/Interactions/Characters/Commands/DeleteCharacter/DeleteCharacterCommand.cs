namespace game_x.application.Features.Interactions.Characters.Commands.DeleteCharacter;

public record DeleteCharacterCommand(Guid Id) : ICommand;
