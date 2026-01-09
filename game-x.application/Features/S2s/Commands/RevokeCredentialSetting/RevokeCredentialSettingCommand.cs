namespace game_x.application.Features.S2s.Commands.RevokeCredentialSetting;

public record RevokeCredentialSettingCommand(string ClientId, string AppCode, string KeyId) : ICommand;
