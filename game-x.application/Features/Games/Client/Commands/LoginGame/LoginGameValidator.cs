using game_x.share.Extensions;
using System.Net;

namespace game_x.application.Features.Games.Client.Commands.LoginGame;

public sealed class LoginGameValidator : AbstractValidator<LoginGameCommand>
{
    private static readonly HashSet<string> AllowedGameCodes =
    [
        "KRF3", "ESF3", "ALPK10", "ESPK10", "LBPK10", "EBPK10", "ANPK6", "M3ANPK6",
        "ESSSC", "ALSSC", "LBSSC", "HKM6", "MCM6", "NMCM6", "TWM6", "HKDM6",
        "FCD3", "PLD3", "ALEGG", "AL10FT", "AL5FT", "BAC001"
    ];
    private static readonly HashSet<string> AllowedLocales = ["zh-Hant", "zh-Hants"];
    private static readonly HashSet<string> AllowedAddress = ["lobby", "lotterygame"];

    public LoginGameValidator()
    {
        RuleFor(x => x.GamePlatformId)
            .NotEmpty().WithMessage($"{nameof(LoginGameCommand.GamePlatformId)} must be not empty.");

        RuleFor(x => x.GameCode)
            .NotEmpty().WithMessage($"{nameof(LoginGameCommand.GameCode)} must be not empty.")
            .Must(code => AllowedGameCodes.Contains(code)).WithMessage($"Invalid {nameof(LoginGameCommand.GameCode)}.");

        RuleFor(x => x.Locale)
            .NotEmpty().WithMessage($"{nameof(LoginGameCommand.Locale)} must be not empty.")
            .Must(locale => AllowedLocales.Contains(locale)).WithMessage($"Invalid {nameof(LoginGameCommand.Locale)}.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage($"{nameof(LoginGameCommand.Address)} must be not empty.")
            .Must(addr => AllowedAddress.Contains(addr)).WithMessage($"Invalid {nameof(LoginGameCommand.Address)}.");

        RuleFor(x => x.ReturnUrl)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidReturnUrl)
            .When(x => x.ReturnUrl.IsNotNullOrEmpty())
            .WithMessage($"{nameof(LoginGameCommand.ReturnUrl)} must be URL-encoded and start with http:// or https://");
    }

    private bool BeValidReturnUrl(string url)
    {
        try
        {
            string decoded = WebUtility.UrlDecode(url);
            if (decoded != url) return false;

            return decoded.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || decoded.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
