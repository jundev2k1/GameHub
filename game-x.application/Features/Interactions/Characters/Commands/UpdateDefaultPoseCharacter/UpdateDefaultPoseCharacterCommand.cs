using game_x.application.Common.Files;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdateDefaultPoseCharacter;

public record UpdateDefaultPoseCharacterCommand(
    [property: JsonIgnore] Guid? Id,
    FileUpload File) : ICommand;
