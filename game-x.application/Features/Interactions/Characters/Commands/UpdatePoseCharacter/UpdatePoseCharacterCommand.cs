using game_x.application.Common.Files;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdatePoseCharacter;

public record UpdatePoseCharacterCommand(
    [property: JsonIgnore] Guid? Id,
    [property: JsonIgnore] Guid? PoseId,
    string Name,
    string Description,
    string Notes,
    FileUpload? File) : ICommand;
