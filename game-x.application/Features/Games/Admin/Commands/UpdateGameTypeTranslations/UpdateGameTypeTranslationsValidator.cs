namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTypeTranslations;

public sealed class UpdateGameTypeTranslationsValidator : AbstractValidator<UpdateGameTypeTranslationsCommand>
{
    public UpdateGameTypeTranslationsValidator()
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
            .SetValidator(new GameTypeTranslationValidator());
    }
}

public class GameTypeTranslationValidator : AbstractValidator<GameTypeTranslationItem>
{
    public GameTypeTranslationValidator()
    {
        RuleFor(x => x.LanguageCode)
            .NotEmpty().WithMessage("Language code is required")
            .Must(LanguageCode.IsValid).WithMessage("The language code '{PropertyValue}' is not supported.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Game name is required")
            .MaximumLength(255).WithMessage("Game name must not exceed {MaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description must not exceed {MaxLength} characters");

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("Notes must not exceed {MaxLength} characters");
    }
}