namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemTranslations;

public sealed class UpdateNavigationItemTranslationsValidator : AbstractValidator<UpdateNavigationItemTranslationsCommand>
{
    public UpdateNavigationItemTranslationsValidator()
    {
        RuleFor(x => x.Translations)
            .Must(x => x.Select(t => t.LanguageCode).Distinct().Count() == x.Length)
            .WithMessage(x => {
                var duplicates = x.Translations
                    .GroupBy(t => t.LanguageCode)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
                return $"Duplicate translations found for the following language codes: {string.Join(", ", duplicates)}.";
            });

        RuleForEach(x => x.Translations)
            .SetValidator(new NavigationTranslationItemValidator());
    }
}

public class NavigationTranslationItemValidator : AbstractValidator<NavigationTranslationItem>
{
    public NavigationTranslationItemValidator()
    {
        RuleFor(x => x.LanguageCode)
            .NotEmpty().WithMessage("Language code is required")
            .Must(LanguageCode.IsValid).WithMessage("The language code '{PropertyValue}' is not supported.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed {MaxLength} characters");
    }
}