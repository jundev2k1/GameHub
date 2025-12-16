using game_x.application.Features.AppSettings.DTOs;

namespace game_x.application.Features.AppSettings.Commands.UpdateSettings;

public record UpdateSettingsCommand(AppSettingInputDto[] Settings) : ICommand;

