using game_x.application.Features.AppSettings.DTOs;

namespace game_x.application.Features.AppSettings.Commands.UpdateSettings;

public sealed class UpdateSettingsValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsValidator()
    {
        RuleFor(x => x.Settings)
            .NotEmpty().WithMessage("Settings is required.")
            .Must(x => x.Select(s => s.Key).Distinct().Count() == x.Length).WithMessage("One or more settings are duplicated.");
        RuleForEach(x => x.Settings)
            .Must(HaveValidAppSetting).WithMessage("One or more settings are invalid.");
    }

    private bool HaveValidAppSetting(AppSettingInputDto setting)
    {
        if (setting.Key is AppSettingConstant.KEY_TALENT_COMMISSION_RATE
            && (!decimal.TryParse(setting.Value, out var rate) || (rate < 0 || rate > 100)))
            return false;

        return true;
    }
}
