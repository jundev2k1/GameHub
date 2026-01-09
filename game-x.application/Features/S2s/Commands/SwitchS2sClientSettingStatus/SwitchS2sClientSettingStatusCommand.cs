namespace game_x.application.Features.S2s.Commands.SwitchS2sClientSettingStatus;

public record SwitchS2sClientSettingStatusCommand(string ClientId, string AppCode) : ICommand;
