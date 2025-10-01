using game_x.application.Common.Files;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Interactions.Characters.Commands.CreatePoseCharacter;

public record CreatePoseCharacterCommand(
    [property: JsonIgnore] Guid? Id,
    string Name,
    string Description,
    string Notes,
    FileUpload File) : ICommand;
