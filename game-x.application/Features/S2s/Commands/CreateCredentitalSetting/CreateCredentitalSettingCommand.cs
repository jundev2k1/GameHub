using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.Commands.CreateCredentitalSetting;

public record CreateCredentitalSettingCommand(
    [property: JsonIgnore] string ClientId,
    [property: JsonIgnore] string AppCode,
    AuthMethod Method,
    CredentialMaterialItem[] Keys) : ICommand;

public record CredentialMaterialItem(CredentialMaterialType Type, string Value, bool IsEncrypted);
