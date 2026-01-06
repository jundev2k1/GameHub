using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.Commands.UpdateS2sClientSetting;

public record UpdateS2sClientSettingCommand(
    [property: JsonIgnore] string ClientId,
    [property: JsonIgnore] string AppCode,
    string AppName,
    string Host,
    string AllowedIps,
    string Notes) : ICommand;
