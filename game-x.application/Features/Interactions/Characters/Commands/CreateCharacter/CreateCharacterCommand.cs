using game_x.application.Common.Files;

namespace game_x.application.Features.Interactions.Characters.Commands.CreateCharacter;

public record CreateCharacterCommand(
    string Name,
    string Description,
    string Notes,
    FileUpload File) : ICommand;
