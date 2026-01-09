namespace game_x.application.Features.S2s.Commands.RotateCredentialSetting;

public record RotateCredentialSettingCommand(string ClientId, string AppCode, string KeyId) : ICommand;
