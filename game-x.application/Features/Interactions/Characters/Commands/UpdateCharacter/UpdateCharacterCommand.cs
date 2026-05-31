using System.Text.Json.Serialization;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdateCharacter;

public record UpdateCharacterCommand(
    [property: JsonIgnore] Guid? Id,
    string? Name,
    string? Description,
    string? Notes) : ICommand;
