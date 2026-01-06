using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.Commands.CreateS2sClientSetting;

public record CreateS2sClientSettingCommand(
    [property: JsonIgnore] string? ClientId,
    string AppName,
    string Host,
    string AllowIps,
    string Notes) : ICommand;
