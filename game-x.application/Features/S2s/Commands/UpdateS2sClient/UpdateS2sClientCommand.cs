using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.Commands.UpdateS2sClient;

public record UpdateS2sClientCommand(
    [property: JsonIgnore] string? ClientId,
    string ClientName,
    string ClientCode,
    string Notes) : ICommand;
