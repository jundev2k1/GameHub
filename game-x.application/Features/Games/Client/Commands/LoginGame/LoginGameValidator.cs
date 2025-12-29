using game_x.share.Extensions;
using System.Net;

namespace game_x.application.Features.Games.Client.Commands.LoginGame;

public sealed class LoginGameValidator : AbstractValidator<LoginGameCommand>
{
    private static readonly HashSet<string> AllowedLocales = ["zh-Hant", "zh-Hants"];
    private static readonly HashSet<string> AllowedAddress = ["lobby", "lotterygame"];

    public LoginGameValidator()
    {
        RuleFor(x => x.GamePlatformId)
            .NotEmpty().WithMessage($"{nameof(LoginGameCommand.GamePlatformId)} must be not empty.");

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
